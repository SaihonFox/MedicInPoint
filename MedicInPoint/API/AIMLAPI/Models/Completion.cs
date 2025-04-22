using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace MedicInPoint.API.AIMLAPI.Models;

public class Completion
{
	public string Model { get; set; } = "gpt-4o";

	public List<Message> Messages { get; set; } = [];

	[JsonProperty("max_tokens")]
	[Range(0, 16384)]
	public int MaxTokens { get; set; } = 16384;

	public bool Stream { get; set; } = false;
}