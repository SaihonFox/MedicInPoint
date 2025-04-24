using System.Globalization;

using Avalonia.Data.Converters;

namespace MedicInPoint.Converters;

public class IntConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is int num && int.TryParse(parameter!.ToString(), out int par))
			return num == par;
		return false;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}