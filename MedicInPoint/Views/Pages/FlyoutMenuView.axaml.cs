using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;

using Lucdem.Avalonia.SourceGenerators.Attributes;

namespace MedicInPoint.Views.Pages;

public partial class FlyoutMenuView : UserControl
{
	[AvaStyledProperty]
	private bool _isOpen = false;

	public FlyoutMenuView()
	{
		InitializeComponent();

		close.Click += (_, _) => IsOpen = false;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (!Design.IsDesignMode)
		{
			IsVisible = IsOpen;
			Opacity = IsOpen ? 1 : 0;
		}

		Logger.Sink?.Log(LogEventLevel.Debug, "FlyoutAREA", this, $"IsOpen: {IsOpen}, Name: {change.Property.Name}");
		Console.WriteLine(change.Property.ToString());
		base.OnPropertyChanged(change);
	}
}