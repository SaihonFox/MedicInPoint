using System.Globalization;

using Avalonia.Data.Converters;

namespace Medic.Theme.Converters;

public sealed class ProgressBarIntermediateOffsetConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		(double)value! * double.Parse($"{parameter}", NumberFormatInfo.InvariantInfo);

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}