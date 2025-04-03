using Avalonia.SimpleRouter;
using MedicInPoint.ViewModels.Pages;

namespace MedicInPoint.ViewModels;

public partial class MainViewModel() : ViewModelBase
{
	public MainViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Main";
		router.GoTo<AuthorizationViewModel, AuthorizationViewModel>();
	}
}