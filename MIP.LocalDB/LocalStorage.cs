namespace MIP.LocalDB;

public class LocalStorage
{
	public static LocalDbContext context = new LocalDbContext().Also(async action => await action.Database.EnsureCreatedAsync());
}

public static class ObjectExtensions
{
	public static T Also<T>(this T obj, Action<T> action)
	{
		action?.Invoke(obj);
		return obj;
	}
}