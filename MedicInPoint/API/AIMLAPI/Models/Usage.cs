using Newtonsoft.Json;

namespace MedicInPoint.API.AIMLAPI.Models;

public class Usage
{
	[JsonProperty("prompt_tokens")]
	public int PromptTokens { get; set; }

	[JsonProperty("completion_tokens")]
	public int CompletionTokens { get; set; }

	[JsonProperty("total_tokens")]
	public int TotalTokens { get; set; }

	[JsonProperty("prompt_tokens_details")]
	public PromptTokensDetails? PromptTokensDetails { get; set; }

	[JsonProperty("completion_tokens_details")]
	public CompletionTokensDetails? CompletionTokensDetails { get; set; }
}