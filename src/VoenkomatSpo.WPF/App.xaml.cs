using System.Windows;
using VoenkomatSpo.Infra;
using VoenkomatSpo.Infra.Authentication;
using VoenkomatSpo.Infra.Documents;
using VoenkomatSpo.WPF.Services;
using VoenkomatSpo.WPF.ViewModels;

namespace VoenkomatSpo.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var authentication = new AuthenticationService(new VoenkomatDbContextFactory(), new Pbkdf2PasswordHasher());
        var login = new LoginViewModel(authentication, new WindowsRememberedCredentialsStore());
        var shell = new ShellViewModel(login, user => new DocumentsViewModel(
            new DocumentService(new VoenkomatDbContextFactory(), new DocumentPdfRenderer(), user),
            new DocumentDesktop(), user, () => login.SignOutCommand.Execute(null)));
        MainWindow = new MainWindow(shell);
        MainWindow.Show();
    }
}
