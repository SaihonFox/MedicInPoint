using Refit;

namespace MedicInPoint.API.Refit;

public class APIService
{
	private static readonly RefitSettings settings = new()
	{
		ContentSerializer = new NewtonsoftJsonContentSerializer(JsonSettings.Settings)
	};

	public static T For<T>() => RestService.For<T>(MedicConfiguration.URL + "api/", settings);
}