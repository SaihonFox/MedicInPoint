using System.Globalization;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

using Medic.Theme.Locale;

namespace Medic.Theme;

public class MedicTheme : Styles
{
	public static readonly Dictionary<CultureInfo, ResourceDictionary> _localeToResource = new()
	{
		{ new CultureInfo("ru-RU"), new ru_ru() },
		{ new CultureInfo("en-US"), new en_us() }
	};

	private static readonly ResourceDictionary _defaultResource = new ru_ru();

	private CultureInfo? _locale;

	public MedicTheme() : this(null) {}

	public MedicTheme(IServiceProvider? provider = null) =>
		AvaloniaXamlLoader.Load(provider, this);

	public CultureInfo? Locale
	{
		get => _locale;
		set
		{
			try
			{
				if (TryGetLocaleResource(value, out var resource) && resource is not null)
				{
					_locale = value;
					foreach (var kv in resource) Resources[kv.Key] = kv.Value;
				}
				else
				{
					_locale = new CultureInfo("ru-RU");
					foreach (var kv in _defaultResource) Resources[kv.Key] = kv.Value;
				}
			}
			catch
			{
				_locale = CultureInfo.InvariantCulture;
			}
		}
	}

	private static bool TryGetLocaleResource(CultureInfo? locale, out ResourceDictionary? resourceDictionary)
	{
		if (Equals(locale, CultureInfo.InvariantCulture))
		{
			resourceDictionary = _defaultResource;
			return true;
		}

		if (locale is null)
		{
			resourceDictionary = _defaultResource;
			return false;
		}

		if (_localeToResource.TryGetValue(locale, out var resource))
		{
			resourceDictionary = resource;
			return true;
		}

		resourceDictionary = _defaultResource;
		return false;
	}

	public static void OverrideLocaleResources(Application application, CultureInfo? culture)
	{
		if (culture is null) return;
		if (!_localeToResource.TryGetValue(culture, out var resources)) return;
		foreach (var kv in resources)
			application.Resources[kv.Key] = kv.Value;
	}

	public static void OverrideLocaleResources(StyledElement element, CultureInfo? culture)
	{
		if (culture is null) return;
		if (!_localeToResource.TryGetValue(culture, out var resources)) return;
		foreach (var kv in resources)
			element.Resources[kv.Key] = kv.Value;
	}

	public static MedicTheme GetInstance(Application app)
	{
		var theme = app.Styles.FirstOrDefault(style => style is MedicTheme);
		if (theme is not MedicTheme medicTheme)
			throw new InvalidOperationException(
				"No MedicTheme instance available. Ensure MedicTheme has been set in Application.Styles in App.axaml.");
		return medicTheme;
	}

	public static MedicTheme GetInstance() => GetInstance(Application.Current!);
}