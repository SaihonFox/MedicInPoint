using System.Globalization;

using Newtonsoft.Json;

namespace MedicInPoint.Converters.Json;

internal class DateOnlyConverter : JsonConverter<DateOnly>
{
	private static readonly string[] formats = [
		"yyyy-MM-dd",
		"yyyy/MM/dd",
		"yyyy.MM.dd",
		"yyyy MM dd",
		"yyyy MMMM dd",
		"dd-MM-yyyy",
		"dd/MM/yyyy",
		"dd.MM.yyyy",
		"dd MM yyyy",
		"dd MMMM yyyy"
	];

	public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String)
		{
			var dateString = reader.Value?.ToString();
			if (
				DateOnly.TryParseExact(dateString, formats, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out DateOnly dateOnly)
			)
				return dateOnly;
		}

		throw new JsonSerializationException($"Unexpected token or cannot parse date. Avaliable formats: {string.Join(", ", formats)}");
	}

	public override async void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer) =>
		await writer.WriteValueAsync(value.ToString("dd-MM-yyyy", CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat));
}