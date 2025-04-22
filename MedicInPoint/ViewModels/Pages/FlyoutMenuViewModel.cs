using Avalonia.Logging;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;
using MedicInPoint.ViewModels.Pages.Admin;

namespace MedicInPoint.ViewModels.Pages;

public partial class FlyoutMenuViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;

	public FlyoutMenuViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		_router = router;
	}

	[RelayCommand]
	private void NavigateTo(string destination)
	{
		switch (destination)
		{
			case "AnalysesAdmin":
				var @goto = _router.GoTo<MenuViewModel, AnalysesAdminViewModel>();
				Logger.Sink?.Log(LogEventLevel.Debug, "FlyoutAREA", @goto.Length, "MSG-TEMPLATE");
				break;
			case "PatientsAdmin":
				break;
			case "UsersAdmin":
				break;
			case "AnalysisCategoriesAdmin":
				break;
		}
	}
}