using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Refit;

namespace MedicInPoint.API.Refit;

public class APIService
{
	private static readonly RefitSettings settings = new()
	{
		ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			NullValueHandling = NullValueHandling.Ignore,
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateFormatString = "dd'-'MM'-'yyyy'T'HH':'mm':'ss.FFFFFFFK",
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
			Converters = [
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				new DateTimeConverter()
			]
		})
	};

	public static T For<T>() => RestService.For<T>(MedicConfiguration.URL + "api/", settings);
}

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

internal class TimeOnlyConverter : JsonConverter<TimeOnly>
{
	public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String)
		{
			var dateString = reader.Value?.ToString();
			if (TimeOnly.TryParseExact(dateString, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
				return dateOnly;
		}

		throw new JsonSerializationException($"Unexpected token or cannot parse time. Avaliable format: HH:mm:ss");
	}

	public override async void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer) =>
		await writer.WriteValueAsync(value.ToString(CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat));
}

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