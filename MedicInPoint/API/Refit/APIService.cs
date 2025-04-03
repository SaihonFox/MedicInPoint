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
			NullValueHandling = NullValueHandling.Ignore
		})
	};

	public static T For<T>() => RestService.For<T>(MedicConfiguration.URL + "api/", settings);
}