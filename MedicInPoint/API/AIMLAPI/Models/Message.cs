namespace MedicInPoint.API.AIMLAPI.Models;

public class Message
{
	public string Role { get; set; } = string.Empty;

	public string Content { get; set; } = string.Empty;

	public object? Refusal { get; set; }

	public List<object> Annotations { get; set; } = [];
}