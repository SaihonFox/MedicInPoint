using Newtonsoft.Json;

namespace MedicInPoint.API.AIMLAPI.Models;

public class CompletionTokensDetails
{
	[JsonProperty("reasoning_tokens")]
	public int ReasoningTokens { get; set; }

	[JsonProperty("audio_tokens")]
	public int AudioTokens { get; set; }

	[JsonProperty("accepted_prediction_tokens")]
	public int AcceptedPredictionTokens { get; set; }

	[JsonProperty("rejected_prediction_tokens")]
	public int RejectedPredictionTokens { get; set; }
}