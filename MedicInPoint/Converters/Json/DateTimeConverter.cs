using System.Globalization;

using Newtonsoft.Json;

namespace MedicInPoint.Converters.Json;

internal class DateTimeConverter : JsonConverter<DateTime>
{
	public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String)
		{
			var dateString = reader.Value?.ToString();
			if (DateTime.TryParseExact(dateString, ["dd-MM-yyyy'T'HH:mm:ssZ", "yyyy-MM-dd'T'HH:mm:ssZ"], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
				return dateTime;
		}

		throw new JsonSerializationException($"Unexpected token or cannot parse datetime. Avaliable formats: dd-MM-yyyy HH:mm:ss, yyyy-MM-dd HH:mm:ss");
	}

	public override async void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer) =>
		await writer.WriteValueAsync(value.ToString(CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat));
}