using System.Globalization;

using Avalonia.Data.Converters;

namespace MedicInPoint.Converters;

public class AnalysisToggleButtonConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool isSelected)
			return isSelected ? "Скрыть" : "Подробнее";
		return "Подробнее";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}