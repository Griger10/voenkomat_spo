using System.Globalization;
using System.Windows.Data;

namespace VoenkomatSpo.WPF.Controls;

public sealed class CurrentPageConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values.Length == 2 && values[0] is int page && values[1] is int current && page == current;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
