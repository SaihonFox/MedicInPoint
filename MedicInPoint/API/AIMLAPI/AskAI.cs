using MedicInPoint.API.AIMLAPI.Models;
using MedicInPoint.API.AIMLAPI.Placeholders;

using Refit;

namespace MedicInPoint.API.AIMLAPI;

public class AskAI
{
	private static readonly RefitSettings settings = new()
	{
		ContentSerializer = new NewtonsoftJsonContentSerializer(JsonSettings.Settings)
	};

	private static T For<T>() => RestService.For<T>(MedicConfiguration.AIMLAPI_URL, settings);

	public static async Task<AIResponse?> SendMessage(string message)
	{
		var completion = new Completion
		{
			Model = "gpt-4o",
			MaxTokens = 16384,
			Messages = [new Message {
				Content = message,
				Role = "user"
			}],
			Stream = false
		};
		var response = await For<IRequest>().PostCompletion(Environment.GetEnvironmentVariable("AIMLAPI_KEY", EnvironmentVariableTarget.Machine)!, completion);
		if (!response.IsSuccessful)
		{
			
			return null;
		}

		return response.Content;
	}
}