using Avalonia.SimpleRouter;

namespace MedicInPoint.ViewModels;

public class LogViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public LogViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		_router = router;
	}
}