using System.Globalization;

using Newtonsoft.Json;

namespace MedicInPoint.Converters.Json;

/*internal class DateTimeConverter : JsonConverter<DateTime>
{
	private static readonly string[] formats = [
		"dd.MM.yyyy'T'HH:mm:ss",
		"dd.MM.yyyy HH:mm:ss",
		"dd.MM.yyyy'T'HH:mm:ss.ffffff",
		"dd.MM.yyyy'T'HH:mm:ss.ffffff zzz",
		"dd.MM.yyyy'T'HH:mm:ss zzz",
		"dd.MM.yyyy'T'H:mm:ss zzz",
		"dd.MM.yyyy H:mm:ss zzz",
	];

	private string writeFormat = "dd.MM.yyyy'T'HH:mm:ss.ffffff";
	
	public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		File.WriteAllText(@"C:\Users\ILNAR\Desktop\datetime read medic.txt", reader.Value!.ToString());
		if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Date)
		{
			var dateString = reader.Value?.ToString();
			if (DateTime.TryParseExact(dateString, formats, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out DateTime dateTime))
				return dateTime;
		}

		string message =
		$"Unexpected token or cannot parse datetime of value: {reader.Value}." +
		$"\nTokenType: {reader.TokenType}" +
		$"\nAvaliable formats: {string.Join(", ", formats)}" +
		$"\nCurrent format: {serializer.DateFormatString}" +
		$"\nPath: {reader.Path}";
		
		File.WriteAllText(@"C:\Users\ILNAR\Desktop\datetime error medic.txt", message);
		throw new JsonSerializationException(message);
	}

	public override async void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
	{
		try
		{
			string val = value.ToString(writeFormat);
			await File.WriteAllTextAsync(
				@"C:\Users\ILNAR\Desktop\datetime write.txt",

				$"Value: {value}\n" +
				$"Format: {serializer.DateFormatString}\n" +
				$"Format2: {writer.DateFormatString}\n" +
				$"Converters: {string.Join(", ", serializer.Converters)}"
			);
			await writer.WriteValueAsync(value);
		}
		catch (JsonWriterException ex)
		{
			await File.WriteAllTextAsync(@"C:\Users\ILNAR\Desktop\ex1.txt", ex.Path);
		}
		catch (ObjectDisposedException ex)
		{
			await File.WriteAllTextAsync(@"C:\Users\ILNAR\Desktop\ex2.txt", ex.Message);
		}
	}
}*/