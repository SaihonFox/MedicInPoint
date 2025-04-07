using System.Globalization;

using Avalonia.Data.Converters;

namespace MedicInPoint.Converters;

public class OpacityByWidthConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is double windowWidth && parameter is string maxWidthString && double.TryParse(maxWidthString, out double maxWidth))
			return windowWidth == maxWidth ? 1 : 0;

		return 1;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}