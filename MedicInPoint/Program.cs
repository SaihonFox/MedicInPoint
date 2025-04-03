using System.Diagnostics;
using System.Text;

using Avalonia;
using Avalonia.Logging;
using Avalonia.Utilities;

namespace MedicInPoint;

internal sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args) => BuildAvaloniaApp()
		.StartWithClassicDesktopLifetime(args);

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.LogToTrace()
			.LogToMySink();
}

static class MyLogExtensions
{
	public static AppBuilder LogToMySink(this AppBuilder builder,
		LogEventLevel level = LogEventLevel.Warning,
		params string[] areas)
	{
		Logger.Sink = new MedicLogSink(level, areas);
		return builder;
	}
}

#pragma warning disable CS9113
class MedicLogSink(LogEventLevel level, params string[] areas) : ILogSink
#pragma warning restore CS9113
{
	public bool IsEnabled(LogEventLevel level, string area) => false;

	public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
	{
		Trace.WriteLine(Format(area, source, messageTemplate));
	}

	public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
	{
		Trace.WriteLine(Format(area, source, messageTemplate, propertyValues));
	}

	private static string Format(
			string area,
			object? source,
			string template,
			object?[]? v = null)
	{
		var result = new StringBuilder(template.Length);
		var r = new CharacterReader(template.AsSpan());
		var i = 0;

		result.Append("[" + DateTime.Now.ToString("H:m:s.fffffff") + " | ");
		result.Append(area);
		result.Append("]-");

		if (source is object)
		{
			result.Append('{');
			result.Append(source.GetType().Name);
			result.Append('#');
			result.Append(source.GetHashCode());
			result.Append("} ");
		}

		while (!r.End)
		{
			var c = r.Take();

			if (c != '{')
			{
				result.Append(c);
			}
			else
			{
				if (r.Peek != '{')
				{
					result.Append('\'');
					result.Append(i < v.Length ? v[i++] : null);
					result.Append('\'');
					r.TakeUntil('}');
					r.Take();
				}
				else
				{
					result.Append('{');
					r.Take();
				}
			}
		}

		return result.ToString();
	}
}