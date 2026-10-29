using System.Globalization;
using System.Windows.Data;
using VoenkomatSpo.Core.Documents;

namespace VoenkomatSpo.WPF.Controls;

public sealed class SelectionLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) => value switch
    {
        StudentOption student => student.Display,
        CommissariatOption commissariat => commissariat.Name,
        KeyValuePair<int, string> form => form.Value,
        KeyValuePair<int?, string> form => form.Value,
        _ => value?.ToString() ?? ""
    };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
}
