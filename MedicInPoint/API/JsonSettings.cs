using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using MedicInPoint.Extensions;

namespace MedicInPoint.API;

public static class JsonSettings
{
	public static JsonSerializerSettings Settings => new JsonSerializerSettings()
	{
		ContractResolver = new CamelCasePropertyNamesContractResolver(),

		Formatting = Formatting.Indented,
		DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		DateParseHandling = DateParseHandling.DateTime,
		//DateFormatString = "dd.MM.yyyy'T'HH:mm:ss.fff",
		Culture = CultureInfo.GetCultureInfo("ru-RU"),
		/*Converters = [
			new DateOnlyConverter(),
			new TimeOnlyConverter(),
			new DateTimeConverter()
		]*/
	}.Also(x => x.Converters.Add(new DateTimeConverter()));
}

internal class DateTimeConverter : JsonConverter<DateTime>
{
	private static readonly string[] formats = [
		"dd.MM.yyyy'T'HH:mm:ss",
		"dd.MM.yyyy HH:mm:ss",
		"dd.MM.yyyy'T'HH:mm:ss.fff",
		"dd.MM.yyyy HH:mm:ss.fff",
		"dd.MM.yyyy'T'HH:mm:ss.fff zzz",
		"dd.MM.yyyy'T'HH:mm:ss zzz",
		"dd.MM.yyyy'T'H:mm:ss zzz",
		"dd.MM.yyyy H:mm:ss zzz",
	];

	private string writeFormat = "dd.MM.yyyy'T'HH:mm:ss.fff";

	public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		//File.WriteAllText(@"C:\Users\ILNAR\Desktop\datetime read api.txt", reader.Value!.ToString());
		if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Date)
		{
			var dateString = reader.Value?.ToString();
			if (DateTime.TryParseExact(dateString, formats, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out DateTime dateTime))
				return dateTime;
		}

		string message = $"Unexpected token or cannot parse datetime of value: {reader.Value}. Avaliable formats: {string.Join(", ", formats)}";

		File.WriteAllText(@"C:\Users\ILNAR\Desktop\datetime error api.txt", message);
		throw new JsonSerializationException(message);
	}

	public override async void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
	{
		await writer.WriteValueAsync(value.ToString(writeFormat));
	}
}