using System.Globalization;

using Newtonsoft.Json;

namespace MedicInPoint.Converters.Json;

internal class TimeOnlyConverter : JsonConverter<TimeOnly>
{
	public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String)
		{
			var dateString = reader.Value?.ToString();
			if (TimeOnly.TryParseExact(dateString, "HH:mm:ss", CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out var dateOnly))
				return dateOnly;
		}

		throw new JsonSerializationException($"Unexpected token or cannot parse time of value: {reader.Value}. Avaliable format: HH:mm:ss");
	}

	public override async void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer) =>
		await writer.WriteValueAsync(value.ToString("HH:mm:ss", CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat));
}