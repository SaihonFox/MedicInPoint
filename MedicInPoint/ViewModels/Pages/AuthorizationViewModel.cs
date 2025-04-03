using Avalonia.SimpleRouter;
using CommunityToolkit.Mvvm.Input;

namespace MedicInPoint.ViewModels.Pages;

public partial class AuthorizationViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public AuthorizationViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Авторизация";
		_router = router;
	}

	[RelayCommand]
	public void Enter()
	{
		_router.GoTo<MenuViewModel>();
	}
}