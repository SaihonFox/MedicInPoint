using System.Net;

using Avalonia.Controls;
using Avalonia.Interactivity;
//using Avalonia.ReactiveUI;
using Avalonia.Threading;

using MedicInPoint.Extensions;
using MedicInPoint.ViewModels.Pages;

//using ReactiveUI;

using Refit;

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

		//enter.Click += enter_Click;
	}

	//public MainWindow TOwner { get => default!; init => TOwner = (MainWindow)Parent!; }

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

	async void enter_Click(object? sender, RoutedEventArgs e)
	{
		if (login.Text.IsNullOrWhiteSpace() || password.Text.IsNullOrWhiteSpace())
		{
			//TOwner.ErrorNotification("������", "���� ������!");
			return;
		}

		enter.IsEnabled = false;
		//User? user = null;
		try
		{
			//user = await APIService.For<IUsers>().GetUser(login.Text, password.Text);
		}
		catch (ValidationApiException ex)
		{
			if (ex.StatusCode != HttpStatusCode.NotFound)
			{
				//TOwner.ErrorNotification("������", ex.StatusCode.ToString());
				enter.IsEnabled = true;
				return;
			}
		}
		catch (Exception ex)
		{
			//TOwner.ErrorNotification("������", ex.StackTrace!);
			enter.IsEnabled = true;
			return;
		}
		/*if (user == null)
		{
			new WindowNotificationManager(this) { Position = NotificationPosition.BottomRight }
				.Show(new Notification("������", "������������ �� ������", NotificationType.Error));
			enter.IsEnabled = true;
			return;
		}*/
		enter.IsEnabled = true;
		//new UserMenuWindow(user).Show();
		//Close();
	}

}