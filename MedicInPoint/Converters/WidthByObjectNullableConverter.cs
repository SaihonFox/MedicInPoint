using System.Globalization;

using Avalonia.Data.Converters;

namespace MedicInPoint.Converters;

public class WidthByObjectNullableConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (parameter is string parameters)
			return value == null ? 0 : double.Parse(parameters);

		return 300;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}