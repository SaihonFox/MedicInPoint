using MedicInPoint.Converters.Json;

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
			/*ContractResolver = new CamelCasePropertyNamesContractResolver(),
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateFormatString = "dd'-'MM'-'yyyy'T'HH':'mm':'ss.FFFFFFF",
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,*/
			Converters = [
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				new DateTimeConverter()
			],
		})
	};

	public static T For<T>() => RestService.For<T>(MedicConfiguration.URL + "api/", settings);
}