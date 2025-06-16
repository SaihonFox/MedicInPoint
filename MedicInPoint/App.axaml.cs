using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.SimpleRouter;

using MedicInPoint.API;
using MedicInPoint.API.SignalR;
using MedicInPoint.Services;
using MedicInPoint.ViewModels;
using MedicInPoint.ViewModels.Pages;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.Pages.Admin.Documents;
using MedicInPoint.ViewModels.Pages.Doctor;

//using HotAvalonia;

using MedicInPoint.Views;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace MedicInPoint;

public partial class App : Application
{
	public static IServiceProvider services { get; private set; } = null!;

	public override void Initialize()
	{
		//this.EnableHotReload();

		JsonConvert.DefaultSettings = () => JsonSettings.Settings;

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
				desktop.MainWindow.Closed += (s, e) => services.GetRequiredService<IAppStateService>().Dispose();
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
		services.AddSingleton(s => new NestedHistoryRouter<ViewModelBase, MainViewModel>(t => (ViewModelBase)s.GetRequiredService(t)));
		services.AddSingleton<INotificationService, NotificationService>();
		services.AddSingleton<MainWindow>();
		services.AddSingleton<MainView>();
		services.AddSingleton<INotificationManager>(c =>
		{
			var mainWindow = c.GetRequiredService<MainWindow>();
			return new WindowNotificationManager(mainWindow)
			{
				Position = NotificationPosition.BottomRight,
				MaxItems = 3
			};
		});

		// Add the ViewModels as a service (Main as singleton, others as transient)
		services.AddSingleton<MainViewModel>();

		services.AddSingleton<LoadingViewModel>();
		services.AddSingleton<SettingsViewModel>();

		services.AddSingleton<AuthorizationViewModel>();
		services.AddSingleton<FlyoutMenuViewModel>();
		services.AddTransient<MenuViewModel>();

		// Admin
		services.AddTransient<AnalysesAdminViewModel>();
		services.AddTransient<UsersAdminViewModel>();
		services.AddTransient<PatientsAdminViewModel>();
		services.AddTransient<AnalysisCategoriesAdminViewModel>();

		// Doctor
		services.AddTransient<PatientDoctorViewModel>();
		services.AddTransient<RnRDoctorViewModel>();

		// Documents
		// Admin
		services.AddTransient<AnalysesAdminDocumentsViewModel>();
		services.AddTransient<PatientsAdminDocumentsViewModel>();
		services.AddTransient<UsersAdminDocumentsViewModel>();

		// Doctor
		//services.AddTransient<AnalysesDoctorDocumentsViewModel>();
		
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