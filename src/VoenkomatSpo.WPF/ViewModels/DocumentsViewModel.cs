using System.Collections.ObjectModel;
using System.Data.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoenkomatSpo.Core.Authentication;
using VoenkomatSpo.Core.Documents;
using VoenkomatSpo.WPF.Services;

namespace VoenkomatSpo.WPF.ViewModels;

public partial class DocumentsViewModel(IDocumentService service, IDocumentDesktop desktop,
    AuthenticatedUser user, Action signOut) : ObservableObject, IDisposable
{
    private CancellationTokenSource? refreshCancellation;
    private readonly CancellationTokenSource lifetime = new();
    private int refreshVersion;
    private bool disposed;
    private bool initialized;
    private PdfFile? generatedPdf;
    public int PageSize { get; private set; } = 4;
    public AuthenticatedUser User => user;
    public ObservableCollection<StudentOption> Students { get; } = [];
    public ObservableCollection<CommissariatOption> Commissariats { get; } = [];
    public ObservableCollection<DocumentRow> Documents { get; } = [];
    public ObservableCollection<int> Pages { get; } = [];
    public IReadOnlyList<int> Years { get; } = Enumerable.Range(2000, DateTime.Today.Year - 2000 + 2).Reverse().ToList();
    public IReadOnlyList<string> Statuses { get; } = ["Все", "Не сверен", "Сверен", "Представлен", "Не представлен"];
    public IReadOnlyList<KeyValuePair<int, string>> Forms { get; } = [new(10, "Форма № 10 (Личная карточка гражданина)"), new(6, "Форма № 6 (Годовой отчёт)")];
    public IReadOnlyList<KeyValuePair<int?, string>> FilterForms { get; } = [new(null, "Все формы"), new(10, "Форма № 10"), new(6, "Форма № 6")];

    [ObservableProperty] private StudentOption? selectedStudent;
    [ObservableProperty] private CommissariatOption? selectedCommissariat;
    [ObservableProperty] private int selectedYear = DateTime.Today.Year;
    [ObservableProperty] private int selectedForm = 10;
    [ObservableProperty] private string search = "";
    [ObservableProperty] private int? filterForm;
    [ObservableProperty] private string filterStatus = "Все";
    [ObservableProperty] private int? filterYear;
    [ObservableProperty] private CommissariatOption? filterCommissariat;
    [ObservableProperty] private bool showFilters;
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool isError;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private bool isWorking;
    [ObservableProperty] private int page = 1;
    [ObservableProperty] private int total;
    [ObservableProperty] private int allCount;
    [ObservableProperty] private int monthCount;
    [ObservableProperty] private int reconciledCount;

    public bool IsForm10 { get => SelectedForm == 10; set { if (value) SelectedForm = 10; } }
    public bool IsForm6 { get => SelectedForm == 6; set { if (value) SelectedForm = 6; } }
    public bool CanEditCards => user.AccessRights.Contains("cards.write");
    public bool CanEditReports => user.AccessRights.Contains("reports.write");
    public bool CanGenerate => !IsWorking && (IsForm10 ? SelectedStudent is not null && CanEditCards : CanEditReports);
    public bool IsIdle => !IsWorking;
    public bool HasDocuments => Documents.Count > 0;
    public string StudentCountLabel => $"{Students.Count} активных карточек";
    public string ContextFitness => $"Категория годности: «{SelectedStudent?.Fitness ?? "—"}»";
    public string ContextCommissariat => $"Военкомат прикрепления: {SelectedStudent?.Commissariat ?? "—"}";
    public string ContextStatus => $"Статус проверки: {(SelectedStudent?.IsChecked == true ? "Проверено" : "Требует проверки")}";
    public string CheckLabel => IsForm10 ? (SelectedStudent?.IsChecked == true ? "Сверка с картотекой ОВК: подтверждена" : "Сведения требуют проверки") : "Сводные данные по выбранному году";
    public string TotalLabel => $"{AllCount} док.";
    public string ConfirmedLabel => AllCount == 0 ? "—" : $"{(double)ReconciledCount / AllCount:P0}";
    public string MonthLabel => $"{MonthCount} записей за текущий месяц";
    public string ReconciliationSummary => AllCount == 0 ? "В журнале пока нет документов" : $"Сверено {ReconciledCount} из {AllCount} документов";
    public string PageLabel => $"Показано {(Total == 0 ? 0 : (Page - 1) * PageSize + 1)}–{Math.Min(Page * PageSize, Total)} из {Total} документов";
    public string Campaign => $"{(DateTime.Today.Month < 7 ? "Весенний" : "Осенний")} призыв {DateTime.Today.Year} / Документы воинского учёта";
    public bool CanPrevious => !IsLoading && Page > 1;
    public bool CanNext => !IsLoading && Page * PageSize < Total;

    public async Task FitPageSizeAsync(double height)
    {
        var size = Math.Clamp((int)Math.Floor((height - 76) / 60), 1, 4);
        if (disposed || PageSize == size) return;
        PageSize = size; Page = 1;
        OnPropertyChanged(nameof(PageSize)); OnPropertyChanged(nameof(PageLabel));
        if (initialized) await RefreshAsync(lifetime.Token);
    }

    partial void OnSelectedFormChanged(int value)
    {
        OnPropertyChanged(nameof(IsForm10)); OnPropertyChanged(nameof(IsForm6)); OnPropertyChanged(nameof(CheckLabel));
        NotifyActions();
    }

    partial void OnSelectedStudentChanged(StudentOption? value)
    {
        OnPropertyChanged(nameof(ContextFitness)); OnPropertyChanged(nameof(ContextCommissariat));
        OnPropertyChanged(nameof(ContextStatus)); OnPropertyChanged(nameof(CheckLabel)); NotifyActions();
    }

    partial void OnIsWorkingChanged(bool value) { OnPropertyChanged(nameof(IsIdle)); NotifyActions(); }
    partial void OnSearchChanged(string value) => _ = DebouncedSearchAsync();

    private async Task DebouncedSearchAsync()
    {
        if (disposed) return;
        refreshCancellation?.Cancel();
        var cancellation = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
        refreshCancellation = cancellation;
        try
        {
            await Task.Delay(300, cancellation.Token);
            Page = 1;
            await RefreshAsync(cancellation.Token);
        }
        catch (OperationCanceledException) { }
        finally { if (ReferenceEquals(refreshCancellation, cancellation)) refreshCancellation = null; cancellation.Dispose(); }
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        try
        {
            var options = await service.GetOptionsAsync(lifetime.Token);
            if (disposed) return;
            Students.Clear(); foreach (var student in options.Students) Students.Add(student);
            Commissariats.Clear(); foreach (var commissariat in options.Commissariats) Commissariats.Add(commissariat);
            SelectedStudent = Students.FirstOrDefault(); SelectedCommissariat = Commissariats.FirstOrDefault();
            FilterCommissariat = Commissariats.FirstOrDefault(); OnPropertyChanged(nameof(StudentCountLabel));
            initialized = true;
            await RefreshAsync(lifetime.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { SetError(ex); }
    }

    [RelayCommand]
    private async Task RefreshAsync() => await RefreshAsync(lifetime.Token);

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var version = ++refreshVersion;
        IsLoading = true;
        try
        {
            var result = await service.GetPageAsync(Query(), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (disposed || version != refreshVersion) return;
            if (Page > 1 && (Page - 1) * PageSize >= result.Total)
            {
                Page = Math.Max(1, (result.Total + PageSize - 1) / PageSize);
                result = await service.GetPageAsync(Query(), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            Documents.Clear(); foreach (var row in result.Rows) Documents.Add(row);
            Total = result.Total; AllCount = result.AllCount; MonthCount = result.MonthCount; ReconciledCount = result.ReconciledCount;
            Pages.Clear();
            var pageCount = Math.Max(1, (Total + PageSize - 1) / PageSize);
            for (var i = Math.Max(1, Page - 2); i <= Math.Min(pageCount, Page + 2); i++) Pages.Add(i);
            foreach (var name in new[] { nameof(HasDocuments), nameof(TotalLabel), nameof(ConfirmedLabel), nameof(MonthLabel), nameof(ReconciliationSummary), nameof(PageLabel), nameof(CanPrevious), nameof(CanNext) })
                OnPropertyChanged(name);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception ex) { SetError(ex); }
        finally { if (version == refreshVersion) { IsLoading = false; OnPropertyChanged(nameof(CanPrevious)); OnPropertyChanged(nameof(CanNext)); } }
    }

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    private async Task GenerateAsync() => await RunAsync(async () =>
    {
        var id = await service.GenerateAsync(new(SelectedForm, SelectedStudent?.Id, SelectedYear, SelectedCommissariat?.Id), lifetime.Token);
        Page = 1;
        await RefreshAsync(lifetime.Token);
        var file = await service.GetPdfAsync(id, lifetime.Token);
        generatedPdf = file;
        SaveGeneratedCommand.NotifyCanExecuteChanged();
        var saved = await desktop.SaveAsync(file);
        Message = saved ? "Документ сформирован и выгружен в PDF." : "Документ сформирован. Нажмите «Сохранить PDF», чтобы выбрать место сохранения.";
        IsError = false;
    });

    private bool CanSaveGenerated() => generatedPdf is not null && !IsWorking;

    [RelayCommand(CanExecute = nameof(CanSaveGenerated))]
    private async Task SaveGeneratedAsync() => await RunAsync(async () =>
    {
        if (generatedPdf is not null && await desktop.SaveAsync(generatedPdf))
        {
            Message = "PDF сохранён.";
            IsError = false;
        }
    });

    [RelayCommand]
    private async Task DownloadAsync(DocumentRow? row)
    {
        if (row is null) return;
        await RunAsync(async () => { if (await desktop.SaveAsync(await service.GetPdfAsync(row.Id, lifetime.Token))) { Message = "PDF сохранён."; IsError = false; } });
    }

    [RelayCommand]
    private async Task OpenAsync(DocumentRow? row)
    {
        if (row is not null) await RunAsync(async () => await desktop.OpenAsync(await service.GetPdfAsync(row.Id, lifetime.Token)));
    }

    [RelayCommand]
    private async Task PrintAsync(DocumentRow? row)
    {
        if (row is not null) await RunAsync(async () => await desktop.OpenAsync(await service.GetPdfAsync(row.Id, lifetime.Token), print: true));
    }

    private bool CanReconcile(DocumentRow? row) => !IsWorking && row?.CanReconcile == true && CanWrite(row);

    [RelayCommand(CanExecute = nameof(CanReconcile))]
    private async Task ReconcileAsync(DocumentRow? row)
    {
        if (row is null || !CanWrite(row) || !desktop.Confirm($"Подтвердить сверку сведений документа {row.Number}?")) return;
        await RunAsync(async () => { await service.ReconcileAsync(row.Id, lifetime.Token); await RefreshAsync(lifetime.Token); Message = "Сверка документа подтверждена."; IsError = false; });
    }

    private bool CanSubmit(DocumentRow? row) => !IsWorking && row?.CanSubmit == true && CanWrite(row);

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task SubmitAsync(DocumentRow? row)
    {
        if (row is null || !CanWrite(row) || !desktop.Confirm($"Документ {row.Number} действительно передан в военкомат? Это действие фиксирует факт передачи, но не отправляет документ.")) return;
        await RunAsync(async () => { await service.SubmitAsync(row.Id, lifetime.Token); await RefreshAsync(lifetime.Token); Message = "Представление документа отмечено."; IsError = false; });
    }

    [RelayCommand]
    private async Task ExportRegistryAsync() => await RunAsync(async () =>
    {
        if (await desktop.SaveAsync(await service.ExportRegistryAsync(Query(), lifetime.Token))) { Message = "Журнал выгружен в PDF с учётом поиска и фильтров."; IsError = false; }
    });

    [RelayCommand]
    private async Task ApplyFiltersAsync() { Page = 1; await RefreshAsync(lifetime.Token); }

    [RelayCommand]
    private async Task ResetFiltersAsync()
    {
        FilterForm = null; FilterStatus = "Все"; FilterYear = null; FilterCommissariat = Commissariats.FirstOrDefault();
        Search = ""; Page = 1; await RefreshAsync(lifetime.Token);
    }

    [RelayCommand] private void ToggleFilters() => ShowFilters = !ShowFilters;
    [RelayCommand] private async Task PreviousAsync() { if (CanPrevious) { Page--; await RefreshAsync(lifetime.Token); } }
    [RelayCommand] private async Task NextAsync() { if (CanNext) { Page++; await RefreshAsync(lifetime.Token); } }
    [RelayCommand] private async Task GoToPageAsync(int value) { Page = value; await RefreshAsync(lifetime.Token); }
    [RelayCommand] private void SignOut() { if (!IsWorking) signOut(); }
    [RelayCommand]
    private void Help() => desktop.ShowInformation("Формирование документов",
        "Форма № 10: выберите студента и сформируйте личную карточку.\nФорма № 6: выберите год и военный комиссариат.\n\nПосле формирования выберите место сохранения PDF. Кнопка «Сохранить PDF» позволяет повторно сохранить последний сформированный файл.\n\nPDF содержит базовые сведения и номер формы. Передача в военкомат выполняется вручную.");
    [RelayCommand] private void Notifications() => desktop.ShowInformation("Состояние документов", $"В журнале: {AllCount}\nСверено: {ReconciledCount}\nТребуют сверки: {AllCount - ReconciledCount}\nЗа текущий месяц: {MonthCount}");
    [RelayCommand] private void Profile() => desktop.ShowInformation("Мои данные", $"Логин: {user.Login}\nРоль: {user.Role}\n\nДоступ: {string.Join(", ", user.AccessRights)}");
    [RelayCommand]
    private void Regulation() => desktop.ShowInformation("Порядок формирования документов",
        "1. Выберите форму № 10 для личной карточки или форму № 6 для годового отчёта.\n2. Укажите студента либо год и военный комиссариат.\n3. Сформируйте и сохраните PDF.\n4. Проверьте документ перед передачей в военный комиссариат.");

    public bool CanWrite(DocumentRow row) => row.FormNumber == 10 ? CanEditCards : CanEditReports;
    private DocumentQuery Query() => new(Search, FilterForm, FilterStatus, FilterYear, FilterCommissariat?.Id, Page, PageSize);
    private void NotifyActions()
    {
        OnPropertyChanged(nameof(CanGenerate)); GenerateCommand.NotifyCanExecuteChanged();
        SaveGeneratedCommand.NotifyCanExecuteChanged();
        ReconcileCommand.NotifyCanExecuteChanged(); SubmitCommand.NotifyCanExecuteChanged();
    }

    private async Task RunAsync(Func<Task> action)
    {
        if (IsWorking || disposed) return;
        IsWorking = true; Message = "";
        try { await action(); }
        catch (OperationCanceledException) { }
        catch (Exception ex) { SetError(ex); }
        finally { IsWorking = false; }
    }

    private void SetError(Exception ex)
    {
        if (disposed) return;
        IsError = true;
        Message = ex switch
        {
            ArgumentException or UnauthorizedAccessException => ex.Message,
            System.IO.FileNotFoundException => ex.Message,
            System.IO.IOException => "Не удалось прочитать или сохранить PDF. Проверьте доступность папки документов.",
            DbException or TimeoutException => "База данных недоступна. Проверьте подключение и нажмите «Обновить».",
            System.ComponentModel.Win32Exception => "Для открытия или печати PDF установите приложение с поддержкой этого действия. PDF можно скачать из журнала.",
            _ => "Не удалось выполнить действие. Проверьте подключение, права доступа и применение миграций, затем повторите попытку."
        };
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true; lifetime.Cancel(); refreshCancellation?.Cancel(); lifetime.Dispose();
    }
}
