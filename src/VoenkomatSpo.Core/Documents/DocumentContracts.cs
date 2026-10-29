namespace VoenkomatSpo.Core.Documents;

public sealed record StudentOption(int Id, string Name, string Group, string FileNumber,
    string Fitness, string Commissariat, bool IsChecked)
{
    public string Display => $"{Name} ({Group}, {FileNumber})";
}

public sealed record CommissariatOption(int? Id, string Name);
public sealed record DocumentOptions(IReadOnlyList<StudentOption> Students, IReadOnlyList<CommissariatOption> Commissariats);
public sealed record DocumentQuery(string Search = "", int? Form = null, string Status = "Все", int? Year = null,
    int? CommissariatId = null, int Page = 1, int PageSize = 4);
public sealed record GenerateDocumentRequest(int FormNumber, int? StudentId, int Year, int? CommissariatId);
public sealed record DocumentRow(int Id, string Number, int FormNumber, int Year, string Subject, string Details,
    string CommissariatName, DateTime FormedAt, string CreatorLogin, bool IsReconciled, DateTime? SubmittedAt)
{
    public string FormLabel => $"Форма № {FormNumber}";
    public string CreatedLabel => FormedAt.ToLocalTime().ToString("dd.MM.yyyy, HH:mm");
    public string StatusLabel => SubmittedAt.HasValue ? "ПРЕДСТАВЛЕН" : IsReconciled ? "СВЕРЕН" : "НЕ СВЕРЕН";
    public string StatusDetail => SubmittedAt.HasValue ? $"Передан: {SubmittedAt.Value.ToLocalTime():dd.MM.yyyy}" : IsReconciled ? "Проверено оператором" : "Ожидает сверки";
    public bool CanSubmit => !SubmittedAt.HasValue && IsReconciled;
    public bool CanReconcile => !IsReconciled && !SubmittedAt.HasValue;
}
public sealed record DocumentPage(IReadOnlyList<DocumentRow> Rows, int Total, int AllCount, int MonthCount, int ReconciledCount);
public sealed record PdfFile(string FileName, byte[] Bytes);

public interface IDocumentService
{
    Task<DocumentOptions> GetOptionsAsync(CancellationToken cancellationToken = default);
    Task<DocumentPage> GetPageAsync(DocumentQuery query, CancellationToken cancellationToken = default);
    Task<int> GenerateAsync(GenerateDocumentRequest request, CancellationToken cancellationToken = default);
    Task<PdfFile> GetPdfAsync(int id, CancellationToken cancellationToken = default);
    Task<PdfFile> ExportRegistryAsync(DocumentQuery query, CancellationToken cancellationToken = default);
    Task ReconcileAsync(int id, CancellationToken cancellationToken = default);
    Task SubmitAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record PdfSection(string Heading, IReadOnlyList<KeyValuePair<string, string>> Fields);
public sealed record PdfContent(string Title, string Subtitle, IReadOnlyList<PdfSection> Sections);

public interface IPdfRenderer
{
    byte[] Render(PdfContent content);
}
