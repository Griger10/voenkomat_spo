using System.IO;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoenkomatSpo.Core.Authentication;

namespace VoenkomatSpo.WPF.ViewModels;

public partial class LoginViewModel(
    IAuthenticationService authentication,
    IRememberedCredentialsStore credentialsStore) : ObservableObject
{
    [ObservableProperty] private string login = "";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPassword))]
    private string password = "";
    [ObservableProperty] private bool isPasswordVisible;
    [ObservableProperty] private bool rememberMe;
    [ObservableProperty] private string errorMessage = "";
    [ObservableProperty] private string sessionNotice = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    [NotifyCanExecuteChangedFor(nameof(SignOutCommand))]
    [NotifyPropertyChangedFor(nameof(SubmitText))]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAuthenticated))]
    [NotifyPropertyChangedFor(nameof(IsAnonymous))]
    [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
    [NotifyCanExecuteChangedFor(nameof(SignOutCommand))]
    private AuthenticatedUser? currentUser;

    public bool HasPassword => Password.Length > 0;
    public bool IsAuthenticated => CurrentUser is not null;
    public bool IsAnonymous => !IsAuthenticated;
    public string SubmitText => IsBusy ? "Выполняется вход…" : "Войти в систему";

    [RelayCommand]
    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var remembered = credentialsStore.Load();
        if (remembered is null || CurrentUser is not null) return;
        Login = remembered.Login;
        Password = remembered.Password;
        RememberMe = true;
        if (!cancellationToken.IsCancellationRequested)
            await SignInCommand.ExecuteAsync(null);
    }

    private bool CanSignIn() => !IsBusy && !IsAuthenticated;

    [RelayCommand(CanExecute = nameof(CanSignIn))]
    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Введите логин и пароль.";
            return;
        }

        IsBusy = true;
        try
        {
            var user = await authentication.SignInAsync(Login, Password, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
            {
                ErrorMessage = "Неверный логин или пароль.";
                TryClearRememberedCredentials();
                Password = "";
                IsPasswordVisible = false;
                return;
            }

            SessionNotice = "";
            try
            {
                if (RememberMe)
                    credentialsStore.Save(new RememberedCredentials(user.Login, Password, DateTime.UtcNow.AddDays(30)));
                else
                    credentialsStore.Clear();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or CryptographicException)
            {
                SessionNotice = "Вход выполнен. Не удалось сохранить настройки входа на устройстве.";
            }

            Login = user.Login;
            Password = "";
            IsPasswordVisible = false;
            CurrentUser = user;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (AuthenticationUnavailableException)
        {
            ErrorMessage = "Не удалось подключиться к базе данных. Проверьте подключение и повторите вход.";
        }
        catch (Exception)
        {
            ErrorMessage = "Не удалось выполнить вход. Проверьте настройки или обратитесь к администратору.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

    private bool CanSignOut() => IsAuthenticated && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanSignOut))]
    private void SignOut()
    {
        TryClearRememberedCredentials();
        CurrentUser = null;
        Password = "";
        IsPasswordVisible = false;
        RememberMe = false;
        ErrorMessage = "";
    }

    private void TryClearRememberedCredentials()
    {
        try { credentialsStore.Clear(); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            SessionNotice = "Не удалось удалить сохранённые данные входа на устройстве.";
        }
    }
}
