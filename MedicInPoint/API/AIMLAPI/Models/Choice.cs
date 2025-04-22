using Newtonsoft.Json;

namespace MedicInPoint.API.AIMLAPI.Models;

public class Choice
{
	public int Index { get; set; }

	[JsonProperty("finish_reason")]
	public string FinishReason { get; set; } = null!;

	public object? Logprobs { get; set; }

	public Message Message { get; set; } = null!;
}