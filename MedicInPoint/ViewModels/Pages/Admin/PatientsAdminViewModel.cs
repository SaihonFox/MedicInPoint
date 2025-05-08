using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class PatientsAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public PatientsAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		_router = router;
		Title = "Список пациентов";
	}

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private Patient? _selectedPatient = null;
}