using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.SimpleRouter;

using MedicInPoint.API.SignalR;
using MedicInPoint.Converters.Json;
using MedicInPoint.Services;
using MedicInPoint.ViewModels;
using MedicInPoint.ViewModels.Pages;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.Pages.Doctor;

//using HotAvalonia;

using MedicInPoint.Views;

using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedicInPoint;

public partial class App : Application
{
	private static IServiceProvider services = null!;

	public override void Initialize()
	{
		//this.EnableHotReload();
		_ = LocalStorage.context;
		JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
			Converters = [
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				new DateTimeConverter()
			],
		};

		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		services = ConfigureServices();

		switch (ApplicationLifetime)
		{
			case IClassicDesktopStyleApplicationLifetime desktop:
				DisableAvaloniaDataAnnotationValidation();
				var mainWindow = services.GetRequiredService<MainWindow>();
				var viewModel = services.GetRequiredService<MainViewModel>();
				mainWindow.DataContext = viewModel;
				desktop.MainWindow = mainWindow;
				break;
			case ISingleViewApplicationLifetime singleViewPlatform:
				var mainView = services.GetRequiredService<MainView>();
				var singleViewModel = services.GetRequiredService<MainViewModel>();
				mainView.DataContext = singleViewModel;
				singleViewPlatform.MainView = mainView;
				break;
		}
		
		base.OnFrameworkInitializationCompleted();
	}

	private static ServiceProvider ConfigureServices()
	{
		var services = new ServiceCollection();

		services.AddSingleton<IAppStateService, AppStateService>();
		services.AddSingleton<MedicSignalRConnections>();

		// Add the HistoryRouter as a service
		services.AddSingleton(s => 
			new NestedHistoryRouter<ViewModelBase, MainViewModel>(t => (ViewModelBase)s.GetRequiredService(t)));
		services.AddSingleton<INotificationService, NotificationService>();
		services.AddSingleton<MainWindow>();
		services.AddSingleton<MainView>();
		services.AddSingleton<INotificationManager>(c =>
		{
			var mainWindow = c.GetRequiredService<MainWindow>();
			return new WindowNotificationManager(mainWindow)
			{
				Position = NotificationPosition.BottomRight,
				MaxItems = 2,
			};
		});

		// Add the ViewModels as a service (Main as singleton, others as transient)
		services.AddSingleton<MainViewModel>();
		services.AddTransient<AuthorizationViewModel>();
		services.AddTransient<FlyoutMenuViewModel>();
		services.AddTransient<MenuViewModel>();

		// Admin
		services.AddTransient<AnalysisAdminViewModel>();
		services.AddTransient<AnalysisCategoriesAdminViewModel>();

		// Doctor
		services.AddTransient<PatientDoctorViewModel>();
		services.AddTransient<RnRDoctorViewModel>();
		
		return services.BuildServiceProvider();
	}

	private void DisableAvaloniaDataAnnotationValidation()
	{
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove)
			BindingPlugins.DataValidators.Remove(plugin);
	}
}