using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class UsersAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public UsersAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		_router = router;
		Title = "Список сотрудников";
	}

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private User? _selectedUser = null;
}