using System.Globalization;
using System.Security.Cryptography;
using System.Text;

using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MedicInPoint.Converters;

public class ColorByNameConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		// Преобразуем текст в массив байтов
		byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(parameter!.ToString()!));

		// Преобразуем первые 6 байтов в шестнадцатеричную строку
		string textHash = System.Convert.ToHexStringLower(bytes);

		// Извлекаем части для красного, зеленого и синего цветов
		int redHex = System.Convert.ToInt32(textHash[..6], 16);
		int greenHex = System.Convert.ToInt32(textHash.Substring(6, 6), 16);
		int blueHex = System.Convert.ToInt32(textHash.Substring(12, 6), 16);

		// Преобразуем шестнадцатеричные значения в диапазон 0-255
		byte red = (byte) (redHex % 256);
		byte green = (byte)(greenHex % 256);
		byte blue = (byte)(blueHex % 256);

		return new SolidColorBrush(new Color(byte.MaxValue, red, green, blue));
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}