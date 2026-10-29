using System.Windows;
using System.Windows.Controls;

namespace VoenkomatSpo.WPF.Controls;

public partial class PasswordInput : UserControl
{
    public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
        nameof(Password), typeof(string), typeof(PasswordInput),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordUpdated));

    public static readonly DependencyProperty IsPasswordVisibleProperty = DependencyProperty.Register(
        nameof(IsPasswordVisible), typeof(bool), typeof(PasswordInput), new PropertyMetadata(false));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public bool IsPasswordVisible
    {
        get => (bool)GetValue(IsPasswordVisibleProperty);
        set => SetValue(IsPasswordVisibleProperty, value);
    }

    public PasswordInput() => InitializeComponent();

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (Password != MaskedPassword.Password)
            SetCurrentValue(PasswordProperty, MaskedPassword.Password);
    }

    private static void OnPasswordUpdated(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var control = (PasswordInput)sender;
        if (control.MaskedPassword is not null && control.MaskedPassword.Password != (string)e.NewValue)
            control.MaskedPassword.Password = (string)e.NewValue;
    }
}
