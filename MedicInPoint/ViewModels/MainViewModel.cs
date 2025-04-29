using System.Diagnostics;

using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;

using MedicInPoint.ViewModels.Pages;

namespace MedicInPoint.ViewModels;

public partial class MainViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public MainViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Main";
		_router = router;

		_router.GoTo<AuthorizationViewModel, AuthorizationViewModel>();
	}

	[RelayCommand]
	private void OpenExplorer() =>
		Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MedicInPoint");

	[RelayCommand]
	private void OpenSettings(bool open)
	{
		if (open)
			_router.GoTo<SettingsViewModel>();
		else
			_router.Back();
	}
}