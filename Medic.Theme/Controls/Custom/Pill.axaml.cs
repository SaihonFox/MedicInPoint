using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

using Lucdem.Avalonia.SourceGenerators.Attributes;

namespace Medic.Theme.Controls.Custom;

public partial class Pill : TemplatedControl
{
#pragma warning disable IDE0044
	public static readonly StyledProperty<bool> IsEditableProperty = AvaloniaProperty.Register<Pill, bool>(
		nameof(IsEditable),
		false,
		defaultBindingMode: BindingMode.TwoWay,
		enableDataValidation: true
	);
	private bool isEditable = false;
	public bool IsEditable
	{
		get => isEditable;
		set => SetValue(IsEditableProperty, value);
	}

	[AvaStyledProperty]
	private string title = string.Empty;
	/*public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<Pill, string>("Title", "");

	public string Title {
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}*/

	public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<Pill, string>(
		nameof(Value),
		string.Empty,
		defaultBindingMode: BindingMode.TwoWay,
		enableDataValidation: true
	);
	private string value = string.Empty;
	public string Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}
	/*public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<Pill, string>("Value", "");

	public string Value
	{
		get => GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}*/

	[AvaStyledProperty]
	private double valueFontSize = 20;
	/*public static readonly StyledProperty<string> ValueFontSizeProperty = AvaloniaProperty.Register<Pill, double>("ValueFontSize", 20);

	public string ValueFontSize
	{
		get => GetValue(ValueFontSizeProperty);
		set => SetValue(ValueFontSizeProperty, value);
	}*/

	[AvaStyledProperty]
	private double titleFontSize = 16;
	/*public static readonly StyledProperty<string> TitleFontSizeProperty = AvaloniaProperty.Register<Pill, double>("TitleFontSize", 16);

	public string TitleFontSize
	{
		get => GetValue(TitleFontSizeProperty);
		set => SetValue(TitleFontSizeProperty, value);
	}*/

	[AvaStyledProperty]
	private IBrush pillBackground = SolidColorBrush.Parse("#F5F5F9");
	[AvaStyledProperty]
	private IBrush valueForeground = Brushes.Black;
	[AvaStyledProperty]
	private IBrush titleForeground = SolidColorBrush.Parse("#939396");

	[AvaStyledProperty]
	private double pillMinWidth = 100;
	/*public static readonly StyledProperty<double> PillMinWidthProperty = AvaloniaProperty.Register<Pill, double>("PillMinWidth", 100);

	public double PillMinWidth
	{
		get => GetValue(PillMinWidthProperty);
		set => SetValue(PillMinWidthProperty, value);
	}*/

	[AvaStyledProperty]
	private double pillMaxWidth = 400;
	/*public static readonly StyledProperty<double> PillMaxWidthProperty = AvaloniaProperty.Register<Pill, double>("PillMaxWidth", 100);

	public double PillMaxWidth
	{
		get => GetValue(PillMaxWidthProperty);
		set => SetValue(PillMaxWidthProperty, value);
	}*/

	[AvaStyledProperty]
	private Thickness pillPadding = new(40, 6);

	/*public static readonly StyledProperty<Thickness> PillPaddingProperty = AvaloniaProperty.Register<Pill, Thickness>("PillPadding", new Thickness(45, 10));

	public Thickness PillPadding
	{
		get => GetValue(PillPaddingProperty);
		set => SetValue(PillPaddingProperty, value);
	}*/
#pragma warning restore IDE0044

	public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent = RoutedEvent.Register<TextBox, TextChangedEventArgs>(nameof(TextChanged), RoutingStrategies.Bubble);

	public event EventHandler<TextChangedEventArgs>? TextChanged
	{
		add => AddHandler(TextChangedEvent, value);
		remove => RemoveHandler(TextChangedEvent, value);
	}

	public Pill()
	{
		
	}
}