using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VoenkomatSpo.Core.Authentication;
using VoenkomatSpo.Core.Documents;
using VoenkomatSpo.Core.Models;

namespace VoenkomatSpo.Infra.Documents;

public sealed class DocumentService(IDbContextFactory<VoenkomatDbContext> factory, IPdfRenderer renderer, AuthenticatedUser actor, string? documentDirectory = null) : IDocumentService
{
    private readonly string documentDirectory = documentDirectory ?? DocumentFiles.DefaultDirectory;
    public async Task<DocumentOptions> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var rights = await RightsAsync(db, cancellationToken);
        Require(rights, "cards.read", "reports.read");
        var cards = await db.PersonalCards.AsNoTracking().Include(x => x.Student).ThenInclude(x => x.Group)
            .Include(x => x.Commissariat).Where(x => !x.IsArchived).OrderBy(x => x.Student.LastName)
            .ThenBy(x => x.Student.FirstName).ToListAsync(cancellationToken);
        var students = cards.Select(x => new StudentOption(x.StudentId, Name(x.Student), x.Student.Group.Name,
            x.Student.PersonalFileNumber, x.FitnessCategory ?? "Не определена", x.Commissariat.Name, x.IsChecked)).ToList();
        var commissariats = await db.MilitaryCommissariats.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new CommissariatOption(x.Id, x.Name)).ToListAsync(cancellationToken);
        commissariats.Insert(0, new(null, "Все военные комиссариаты"));
        return new(students, commissariats);
    }

    public async Task<DocumentPage> GetPageAsync(DocumentQuery query, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var rights = await RightsAsync(db, cancellationToken);
        var all = Readable(db, rights);
        var count = await all.CountAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var month = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthly = await all.CountAsync(x => x.FormedAt >= month, cancellationToken);
        var reconciled = await all.CountAsync(x => x.IsReconciled, cancellationToken);
        var filtered = Apply(all, query);
        var total = await filtered.CountAsync(cancellationToken);
        var rows = await Rows(filtered.OrderByDescending(x => x.FormedAt).ThenByDescending(x => x.Id)
            .Skip((Math.Max(1, query.Page) - 1) * Math.Clamp(query.PageSize, 1, 100)).Take(Math.Clamp(query.PageSize, 1, 100)))
            .ToListAsync(cancellationToken);
        return new(rows, total, count, monthly, reconciled);
    }

    public async Task<int> GenerateAsync(GenerateDocumentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FormNumber is not (6 or 10)) throw new ArgumentException("Выберите форму № 6 или № 10.");
        if (request.Year < 2000 || request.Year > DateTime.Today.Year + 1) throw new ArgumentException("Некорректный отчётный год.");
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var rights = await RightsAsync(db, cancellationToken);
        Require(rights, request.FormNumber == 10 ? "cards.write" : "reports.write");
        var now = DateTime.UtcNow;
        var number = $"{(request.FormNumber == 10 ? "ЛК" : "ОТ")}-{request.Year}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var document = new GeneratedDocument
        {
            Number = number, FormNumber = request.FormNumber, Year = request.Year,
            FormedAt = now, FormedBy = actor.Id, CreatorLogin = actor.Login
        };
        PdfContent content;
        if (request.FormNumber == 10)
        {
            var card = await db.PersonalCards.AsNoTracking().Include(x => x.Student).ThenInclude(x => x.Group)
                .Include(x => x.Student).ThenInclude(x => x.City).Include(x => x.Commissariat)
                .SingleOrDefaultAsync(x => x.StudentId == request.StudentId && !x.IsArchived, cancellationToken)
                ?? throw new ArgumentException("Выберите студента с действующей личной карточкой.");
            document.StudentId = card.StudentId;
            document.CommissariatId = card.CommissariatId;
            document.CommissariatName = card.Commissariat.Name;
            document.Subject = Name(card.Student);
            document.Details = $"Группа {card.Student.Group.Name} • {card.Student.PersonalFileNumber}";
            document.IsReconciled = card.IsChecked;
            var student = card.Student;
            content = new("Форма № 10", $"Личная карточка гражданина • № {number}",
            [
                Section("Реквизиты документа", ("Сформирован", Local(now)), ("Сформировал", actor.Login), ("Военный комиссариат", card.Commissariat.Name)),
                Section("Сведения о студенте", ("ФИО", Name(student)), ("Дата рождения", student.BirthDate.ToString("dd.MM.yyyy")),
                    ("Личное дело", student.PersonalFileNumber), ("Группа", student.Group.Name), ("Отделение", student.Group.Department),
                    ("Год выпуска", student.Group.GraduationYear.ToString()), ("Дата зачисления", student.EnrolledAt.ToString("dd.MM.yyyy")),
                    ("Статус обучения", student.Status), ("Адрес", $"{student.PostIndex}, {student.City.Name}, {student.Street}, д. {student.HouseNumber}"),
                    ("Паспорт", student.PassportData)),
                Section("Воинский учёт", ("Категория учёта", card.RecordCategory), ("Категория годности", card.FitnessCategory ?? "Не определена"),
                    ("Военно-учётная специальность", card.MilitarySpecialty ?? "Не указана"), ("Документ", card.DocumentType),
                    ("Номер документа", card.DocumentNumber ?? "Не представлен"), ("Дата выдачи", card.DocumentIssuedAt?.ToString("dd.MM.yyyy") ?? "Не указана"),
                    ("Проверка сведений", card.IsChecked ? "Проверено" : "Требует проверки"))
            ]);
        }
        else
        {
            var commissariat = request.CommissariatId.HasValue
                ? await db.MilitaryCommissariats.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.CommissariatId, cancellationToken)
                    ?? throw new ArgumentException("Военный комиссариат не найден.") : null;
            var yearEnd = new DateOnly(request.Year, 12, 31);
            var cards = await db.PersonalCards.AsNoTracking().Include(x => x.Student).ThenInclude(x => x.Group).Include(x => x.Commissariat)
                .Where(x => x.Student.EnrolledAt <= yearEnd && (x.RemovedAt == null || x.RemovedAt > yearEnd)
                    && (!x.IsArchived || x.RemovedAt > yearEnd)
                    && (!request.CommissariatId.HasValue || x.CommissariatId == request.CommissariatId))
                .OrderBy(x => x.Commissariat.Name).ThenBy(x => x.Student.LastName).ToListAsync(cancellationToken);
            document.CommissariatId = request.CommissariatId;
            document.CommissariatName = commissariat?.Name ?? "Все военные комиссариаты";
            document.Subject = $"{request.Year} год";
            document.Details = $"Годовая отчётность • {cards.Count} чел.";
            document.IsReconciled = cards.Count > 0 && cards.All(x => x.IsChecked);
            if (commissariat is not null)
                document.Report = new Report { Year = request.Year, FormedAt = now, FormedBy = actor.Id,
                    CommissariatId = commissariat.Id, IsReconciled = document.IsReconciled };
            var sections = new List<PdfSection>
            {
                Section("Параметры отчёта", ("Отчётный год", request.Year.ToString()), ("Военный комиссариат", document.CommissariatName),
                    ("Сформирован", Local(now)), ("Сформировал", actor.Login), ("Всего на воинском учёте", cards.Count.ToString()),
                    ("В запасе / военнообязанные", cards.Count(x => x.RecordCategory.Contains("запас", StringComparison.OrdinalIgnoreCase) || x.RecordCategory.Contains("Военнообязан", StringComparison.OrdinalIgnoreCase)).ToString()),
                    ("Призывники", cards.Count(x => x.RecordCategory == "Призывник").ToString()),
                    ("Сведения проверены", cards.Count(x => x.IsChecked).ToString()), ("Требуют проверки", cards.Count(x => !x.IsChecked).ToString())),
                new("Распределение по военным комиссариатам", cards.GroupBy(x => x.Commissariat.Name).Select(x => new KeyValuePair<string, string>(x.Key, $"{x.Count()} чел.")).ToList())
            };
            foreach (var card in cards)
                sections.Add(Section(Name(card.Student), ("Личное дело / группа", $"{card.Student.PersonalFileNumber} / {card.Student.Group.Name}"),
                    ("Категория учёта / годности", $"{card.RecordCategory} / {card.FitnessCategory ?? "Не определена"}"), ("Военный комиссариат", card.Commissariat.Name)));
            content = new("Форма № 6", $"Годовой отчёт по воинскому учёту за {request.Year} год • № {number}", sections);
        }
        var pdf = await Task.Run(() => renderer.Render(content), cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(this.documentDirectory);
        document.PdfPath = Guid.NewGuid().ToString("N") + ".pdf";
        var path = DocumentFiles.Resolve(this.documentDirectory, document.PdfPath);
        try
        {
            await File.WriteAllBytesAsync(path, pdf, cancellationToken);
            db.GeneratedDocuments.Add(document);
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            db.ActionLogs.Add(Audit("Формирование документа", document.Id));
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return document.Id;
        }
        catch
        {
            try { File.Delete(path); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            throw;
        }
    }

    public async Task<PdfFile> GetPdfAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var rights = await RightsAsync(db, cancellationToken);
        var doc = await Readable(db, rights).AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ArgumentException("Документ не найден или недоступен.");
        var path = DocumentFiles.Resolve(documentDirectory, doc.PdfPath);
        if (!File.Exists(path)) throw new FileNotFoundException("Файл документа не найден в хранилище. Восстановите файл из резервной копии.");
        return new($"Form{doc.FormNumber}_{doc.Id}_{doc.Year}.pdf", await File.ReadAllBytesAsync(path, cancellationToken));
    }

    public async Task<PdfFile> ExportRegistryAsync(DocumentQuery query, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var rights = await RightsAsync(db, cancellationToken);
        var rows = await Rows(Apply(Readable(db, rights), query).OrderByDescending(x => x.FormedAt)).ToListAsync(cancellationToken);
        var sections = rows.Select(x => Section($"{x.Number} • {x.FormLabel}", ("Студент / период", x.Subject),
            ("Сведения", x.Details), ("Военный комиссариат", x.CommissariatName), ("Сформирован", x.CreatedLabel),
            ("Сформировал", x.CreatorLogin), ("Сверка", x.IsReconciled ? "Сверен" : "Не сверен"),
            ("Представление", x.SubmittedAt.HasValue ? Local(x.SubmittedAt.Value) : "Не представлен"))).ToList();
        sections.Insert(0, Section("Параметры выгрузки", ("Всего документов", rows.Count.ToString()), ("Поиск", query.Search),
            ("Форма", query.Form?.ToString() ?? "Все"), ("Статус", query.Status), ("Год", query.Year?.ToString() ?? "Все"), ("Сформировано", Local(DateTime.UtcNow))));
        var bytes = await Task.Run(() => renderer.Render(new("Журнал документов", "Формы № 6 и № 10", sections)), cancellationToken);
        return new($"DocumentRegistry_{DateTime.Today:yyyyMMdd}.pdf", bytes);
    }

    public Task ReconcileAsync(int id, CancellationToken cancellationToken = default) => UpdateStatusAsync(id, false, cancellationToken);
    public Task SubmitAsync(int id, CancellationToken cancellationToken = default) => UpdateStatusAsync(id, true, cancellationToken);

    private async Task UpdateStatusAsync(int id, bool submit, CancellationToken cancellationToken)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var rights = await RightsAsync(db, cancellationToken);
        var doc = await db.GeneratedDocuments.Include(x => x.Report).SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ArgumentException("Документ не найден.");
        Require(rights, doc.FormNumber == 10 ? "cards.write" : "reports.write");
        if (submit)
        {
            if (doc.SubmittedAt.HasValue) return;
            if (!doc.IsReconciled) throw new ArgumentException("Сначала выполните сверку документа.");
            doc.SubmittedAt = DateTime.UtcNow;
            if (doc.Report is not null) doc.Report.SubmittedAt = doc.SubmittedAt;
        }
        else
        {
            if (doc.IsReconciled) return;
            doc.IsReconciled = true;
            if (doc.Report is not null) doc.Report.IsReconciled = true;
        }
        db.ActionLogs.Add(Audit(submit ? "Представление документа в ВК" : "Сверка документа", id));
        try { await db.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Статус документа уже изменён другим пользователем. Обновите журнал.");
        }
    }

    private async Task<string[]> RightsAsync(VoenkomatDbContext db, CancellationToken cancellationToken)
    {
        var rights = await db.Users.AsNoTracking().Where(x => x.Id == actor.Id).Select(x => x.Role.AccessRights).SingleOrDefaultAsync(cancellationToken)
            ?? throw new UnauthorizedAccessException("Пользователь больше не имеет доступа к системе.");
        return rights.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    private static void Require(string[] rights, params string[] any)
    {
        if (!any.Any(rights.Contains)) throw new UnauthorizedAccessException("Недостаточно прав для этого действия.");
    }

    private static IQueryable<GeneratedDocument> Readable(VoenkomatDbContext db, string[] rights)
    {
        Require(rights, "cards.read", "reports.read");
        var cards = rights.Contains("cards.read");
        var reports = rights.Contains("reports.read");
        return db.GeneratedDocuments.Where(x => (cards && x.FormNumber == 10) || (reports && x.FormNumber == 6));
    }

    private static IQueryable<GeneratedDocument> Apply(IQueryable<GeneratedDocument> q, DocumentQuery filter)
    {
        var search = filter.Search.Trim();
        if (search.Length > 0)
        {
            var pattern = "%" + search.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
            q = q.Where(x => EF.Functions.ILike(x.Subject, pattern, "\\") || EF.Functions.ILike(x.Number, pattern, "\\") || EF.Functions.ILike(x.Details, pattern, "\\"));
        }
        if (filter.Form.HasValue) q = q.Where(x => x.FormNumber == filter.Form);
        if (filter.Year.HasValue) q = q.Where(x => x.Year == filter.Year);
        if (filter.CommissariatId.HasValue) q = q.Where(x => x.CommissariatId == filter.CommissariatId);
        return filter.Status switch
        {
            "Не сверен" => q.Where(x => !x.IsReconciled),
            "Сверен" => q.Where(x => x.IsReconciled),
            "Представлен" => q.Where(x => x.SubmittedAt != null),
            "Не представлен" => q.Where(x => x.SubmittedAt == null),
            _ => q
        };
    }

    private static IQueryable<DocumentRow> Rows(IQueryable<GeneratedDocument> q) => q.Select(x => new DocumentRow(
        x.Id, x.Number, x.FormNumber, x.Year, x.Subject, x.Details, x.CommissariatName, x.FormedAt, x.CreatorLogin, x.IsReconciled, x.SubmittedAt));

    private ActionLog Audit(string action, int id) => new() { Action = action, TableName = "GeneratedDocument", RecordId = id, RecordedAt = DateTime.UtcNow, UserId = actor.Id };
    private static string Name(Student student) => string.Join(" ", new[] { student.LastName, student.FirstName, student.Patronymic }.Where(x => !string.IsNullOrWhiteSpace(x)));
    private static string Local(DateTime date) => date.ToLocalTime().ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
    private static PdfSection Section(string title, params (string Key, string Value)[] fields) => new(title, fields.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList());
}
