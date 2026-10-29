using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using VoenkomatSpo.Core.Authentication;

namespace VoenkomatSpo.WPF.ViewModels;

public partial class ShellViewModel : ObservableObject, IDisposable
{
    private readonly Func<AuthenticatedUser, DocumentsViewModel> createDocuments;
    public LoginViewModel Login { get; }
    [ObservableProperty] private DocumentsViewModel? documents;

    public ShellViewModel(LoginViewModel login, Func<AuthenticatedUser, DocumentsViewModel> createDocuments)
    {
        Login = login;
        this.createDocuments = createDocuments;
        Login.PropertyChanged += LoginChanged;
    }

    private async void LoginChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(LoginViewModel.CurrentUser)) return;
        Documents?.Dispose();
        Documents = Login.CurrentUser is null ? null : createDocuments(Login.CurrentUser);
        if (Documents is not null) await Documents.InitializeCommand.ExecuteAsync(null);
    }

    public void Dispose()
    {
        Login.PropertyChanged -= LoginChanged;
        Login.InitializeCommand.Cancel(); Login.SignInCommand.Cancel(); Documents?.Dispose();
    }
}
