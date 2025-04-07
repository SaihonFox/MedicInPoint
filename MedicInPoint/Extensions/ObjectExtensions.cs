namespace MedicInPoint.Extensions;

public static class ObjectExtensions
{
	public static T Also<T>(this T obj, Action<T> action)
	{
		action?.Invoke(obj);
		return obj;
	}

	public static R Let<T, R>(this T obj, Func<T, R> block) => block(obj);
}