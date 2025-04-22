using Avalonia.SimpleRouter;

namespace MedicInPoint.ViewModels.Pages;

public class LoadingViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public LoadingViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Окно загрузки";
		_router = router;
	}
}