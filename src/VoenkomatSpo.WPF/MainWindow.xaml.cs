using System.Windows;
using System.ComponentModel;
using VoenkomatSpo.WPF.ViewModels;

namespace VoenkomatSpo.WPF;

public partial class MainWindow : Window
{
    private readonly ShellViewModel viewModel;

    public MainWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = viewModel;
        viewModel.Login.PropertyChanged += LoginChanged;
        Loaded += async (_, _) => await viewModel.Login.InitializeCommand.ExecuteAsync(null);
    }

    private void LoginChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(LoginViewModel.CurrentUser)) return;
        var area = SystemParameters.WorkArea;
        var documents = viewModel.Login.IsAuthenticated;
        MinWidth = Math.Min(documents ? 1180 : 580, area.Width);
        MinHeight = Math.Min(documents ? 820 : 600, area.Height);
        Width = Math.Min(documents ? 1440 : 1296, area.Width);
        Height = Math.Min(documents ? 960 : 881, area.Height);
        Left = area.Left + (area.Width - Width) / 2;
        Top = area.Top + (area.Height - Height) / 2;
    }

    protected override void OnClosed(EventArgs e)
    {
        viewModel.Login.PropertyChanged -= LoginChanged;
        viewModel.Dispose();
        base.OnClosed(e);
    }
}
