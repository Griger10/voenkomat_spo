using System.Windows;
using VoenkomatSpo.WPF.ViewModels;

namespace VoenkomatSpo.WPF;

public partial class MainWindow : Window
{
    private readonly LoginViewModel viewModel;

    public MainWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.InitializeCommand.ExecuteAsync(null);
    }

    protected override void OnClosed(EventArgs e)
    {
        viewModel.InitializeCommand.Cancel();
        viewModel.SignInCommand.Cancel();
        base.OnClosed(e);
    }
}
