namespace MedicInPoint.Extensions;

public static class StringExtensions
{
	public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

	public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);
}