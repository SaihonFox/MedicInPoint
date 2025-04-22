using Newtonsoft.Json;

namespace MedicInPoint.API.AIMLAPI.Models;

public class PromptTokensDetails
{
	[JsonProperty("cached_tokens")]
	public int CachedTokens { get; set; }

	[JsonProperty("audio_tokens")]
	public int AudioTokens { get; set; }
}