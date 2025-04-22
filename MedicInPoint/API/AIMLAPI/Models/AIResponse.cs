using Newtonsoft.Json;

namespace MedicInPoint.API.AIMLAPI.Models;

public class AIResponse
{
	public string Id { get; set; } = null!;

	public string Object { get; set; } = null!;

	public List<Choice> choices { get; set; } = [];

	public long Created { get; set; }

	public string Model { get; set; } = null!;

	public Usage? Usage { get; set; }

	[JsonProperty("system_fingerprint")]
	public string SystemFingerprint { get; set; } = null!;
}