using Newtonsoft.Json;

namespace MedicInPoint.Extensions;

public static class HttpResponseMessageExtensions
{
	public static async Task<T?> GetContentAs<T>(this HttpResponseMessage response)
	{
		var json = await response.Content.ReadAsStringAsync();
		return JsonConvert.DeserializeObject<T>(json);
	}
}