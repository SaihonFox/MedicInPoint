using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace MedicInPoint.Views.Pages;

public partial class AuthorizationView : UserControl
{
	public System.Timers.Timer timer = new(1000);
	public int itimer;

	public AuthorizationView()
	{
		InitializeComponent();

		SetTimer();

		onboard1.Click += Onboard_Click;
		onboard2.Click += Onboard_Click;
		onboard3.Click += Onboard_Click;

		Loaded += (_, _) =>
			login.Focus(NavigationMethod.Unspecified);
	}

	private void Onboard_Click(object? sender, RoutedEventArgs e)
	{
		itimer = 0;
		string onboard_name = (sender as RadioButton)!.Name!;
		slides.SelectedIndex = int.Parse(onboard_name[^1].ToString()) - 1;
	}

	void SetTimer()
	{
		timer.Elapsed += async (_, _) =>
		{
			itimer++;
			if (itimer != 5) return;
			itimer = 0;

			await Dispatcher.UIThread.InvokeAsync(() => slides.SelectedIndex = slides.SelectedIndex + 1 == 3 ? 0 : slides.SelectedIndex + 1);
			await Dispatcher.UIThread.InvokeAsync(() => this.FindControl<RadioButton>("onboard" + (slides.SelectedIndex + 1))!.IsChecked = true);
		};
		timer.Start();
	}
}