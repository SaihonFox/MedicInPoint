using System.Globalization;

using Newtonsoft.Json;

namespace MedicInPoint.Converters.Json;

internal class DateTimeConverter : JsonConverter<DateTime>
{
	private static readonly string[] formats = [
		"dd.MM.yyyy'T'HH:mm:ss",
		"dd.MM.yyyy HH:mm:ss",
		"dd.MM.yyyy'T'HH:mm:ssZ",
		"dd.MM.yyyy HH:mm:ssZ",
		"dd.M.yyyy'T'HH:mm:ss",
		"d.MM.yyyy'T'HH:mm:ss",
		"d.M.yyyy'T'HH:mm:ss",
		"dd-MM-yyyy'T'HH:mm:ss",
		"dd-M-yyyy'T'HH:mm:ss",
		"d-MM-yyyy'T'HH:mm:ss",
		"d-M-yyyy'T'HH:mm:ss",

		"dd.MM.yyyy'T'HH:mm:ss zzz",
		"dd.MM.yyyy HH:mm:ss zzz",
		"dd.MM.yyyy'T'HH:mm:ss zzz",
		"dd.MM.yyyy HH:mm:ss zzz",
		"dd.M.yyyy'T'HH:mm:ss zzz",
		"d.MM.yyyy'T'HH:mm:ss zzz",
		"d.M.yyyy'T'HH:mm:ss zzz",
		"dd-MM-yyyy'T'HH:mm:ss zzz",
		"dd-M-yyyy'T'HH:mm:ss zzz",
		"d-MM-yyyy'T'HH:mm:ss zzz",
		"d-M-yyyy'T'HH:mm:ss zzz",

		"dd.MM.yyyy'T'HH:mm:ss.ffffff zzz",
		"dd.MM.yyyy HH:mm:ss.ffffff zzz",
		"dd.MM.yyyy'T'HH:mm:ss.ffffff zzz",
		"dd.MM.yyyy HH:mm:ss.ffffff zzz",
		"dd.M.yyyy'T'HH:mm:ss.ffffff zzz",
		"d.MM.yyyy'T'HH:mm:ss.ffffff zzz",
		"d.M.yyyy'T'HH:mm:ss.ffffff zzz",
		"dd-MM-yyyy'T'HH:mm:ss.ffffff zzz",
		"dd-M-yyyy'T'HH:mm:ss.ffffff zzz",
		"d-MM-yyyy'T'HH:mm:ss.ffffff zzz",
		"d-M-yyyy'T'HH:mm:ss.ffffff zzz"
	];

	private string writeFormat = "dd.MM.yyyy'T'HH:mm:ss.ffffff zzz";

	public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Date)
		{
			var dateString = reader.Value?.ToString();
			if (DateTime.TryParseExact(dateString, formats, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out DateTime dateTime))
				return dateTime;
		}

		throw new JsonSerializationException($"Unexpected token or cannot parse datetime of value: {reader.Value}. Avaliable formats: {string.Join(", ", formats)}");
	}

	public override async void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer) =>
		await writer.WriteValueAsync(value.ToString(writeFormat, CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat));
}