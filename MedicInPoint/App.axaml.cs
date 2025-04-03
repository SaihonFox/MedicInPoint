using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.SimpleRouter;


using MedicInPoint.ViewModels;
using MedicInPoint.ViewModels.Pages;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.Pages.Doctor;


//using HotAvalonia;

using MedicInPoint.Views;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint;

public partial class App : Application
{
	public static IServiceProvider services = null!;

	public override void Initialize()
	{
		//this.EnableHotReload();
		AvaloniaXamlLoader.Load(this);
		
		/*Navigation.UIPlatform.RegisterPage(typeof(AuthorizationView));

		var mainStack = new NavigationPageStack(Navigation.MainStackName, "Medic");
		Navigation.UIPlatform.AddStack(mainStack);

		mainStack.AddPage<AuthorizationView>(string.Empty);*/
	}

	public override void OnFrameworkInitializationCompleted()
	{
		services = ConfigureServices();
		var mainViewModel = services.GetRequiredService<MainViewModel>();

		switch (ApplicationLifetime)
		{
			case IClassicDesktopStyleApplicationLifetime desktop:
				DisableAvaloniaDataAnnotationValidation();
				desktop.MainWindow = new MainWindow { DataContext = mainViewModel };
				break;
			case ISingleViewApplicationLifetime singleViewPlatform:
				singleViewPlatform.MainView = new MainView { DataContext = mainViewModel };
				break;
		}
		
		base.OnFrameworkInitializationCompleted();
	}

	private static ServiceProvider ConfigureServices()
	{
		var services = new ServiceCollection();
		// Add the HistoryRouter as a service
		services.AddSingleton(s => 
			new NestedHistoryRouter<ViewModelBase, MainViewModel>( t => (ViewModelBase)s.GetRequiredService(t)));

		// Add the ViewModels as a service (Main as singleton, others as transient)
		services.AddSingleton<MainViewModel>();
		services.AddTransient<AuthorizationViewModel>();
		services.AddTransient<FlyoutMenuViewModel>();
		services.AddTransient<MenuViewModel>();

		// Admin
		services.AddTransient<AnalysisAdminViewModel>();
		services.AddTransient<AnalysisCategoriesAdminViewModel>();

		// Patient
		services.AddTransient<PatientDoctorViewModel>();
		
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