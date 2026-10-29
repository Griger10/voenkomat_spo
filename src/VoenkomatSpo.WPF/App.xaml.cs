using System.Windows;
using VoenkomatSpo.Infra;
using VoenkomatSpo.Infra.Authentication;
using VoenkomatSpo.WPF.ViewModels;

namespace VoenkomatSpo.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var authentication = new AuthenticationService(new VoenkomatDbContextFactory(), new Pbkdf2PasswordHasher());
        MainWindow = new MainWindow(new LoginViewModel(authentication, new WindowsRememberedCredentialsStore()));
        MainWindow.Show();
    }
}
