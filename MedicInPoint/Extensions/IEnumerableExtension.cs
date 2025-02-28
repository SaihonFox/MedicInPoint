using System.Collections;

namespace MedicInPoint.Extensions;

public static class IEnumerableExtension
{
	public static string Join(this IEnumerable values, string separator) => string.Join(separator, values);
}