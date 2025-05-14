using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;
using MedicInPoint.Services;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class PatientsAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly IAppStateService _appService = null!;

	public PatientsAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, IAppStateService appService) : this()
	{
		_router = router;
		Title = "Список пациентов";
		_appService = appService;
	}

	[RelayCommand]
	private void Back() => _router.Back();

	public string CurrentUser => _appService.CurrentUser!.FullName;

	[ObservableProperty]
	private Patient? _selectedPatient = null;
}