using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedicInPoint.API;

public static class JsonSettings
{
	public static JsonSerializerSettings Settings => new JsonSerializerSettings
	{
		ContractResolver = new CamelCasePropertyNamesContractResolver(),
		
		Formatting = Formatting.Indented,
		DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		DateParseHandling = DateParseHandling.DateTime,
		//DateFormatString = "dd.MM.yyyy'T'HH:mm:ss.fff",
		Culture = CultureInfo.GetCultureInfo("ru-RU"),
		DateTimeZoneHandling = DateTimeZoneHandling.Local,
		/*Converters = [
			new DateOnlyConverter(),
			new TimeOnlyConverter(),
			new DateTimeConverter()
		]*/
	};
}