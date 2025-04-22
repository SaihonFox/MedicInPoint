using System.Globalization;

using MedicInPoint.Converters.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedicInPoint.API;

public static class JsonSettings
{
	public static JsonSerializerSettings Settings => new JsonSerializerSettings
	{
		ContractResolver = new CamelCasePropertyNamesContractResolver(),
		NullValueHandling = NullValueHandling.Include,
		Formatting = Formatting.Indented,
		DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		MissingMemberHandling = MissingMemberHandling.Ignore,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		DateParseHandling = DateParseHandling.DateTimeOffset,
		DateFormatHandling = DateFormatHandling.IsoDateFormat,
		DateFormatString = @"dd.MM.yyyy'T'HH:mm:ss Z",
		Culture = CultureInfo.GetCultureInfo("ru-RU"),
		DateTimeZoneHandling = DateTimeZoneHandling.Utc,
		Converters = [
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				new DateTimeConverter()
			]
	};
}