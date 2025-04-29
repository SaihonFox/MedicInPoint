using Avalonia.SimpleRouter;

namespace MedicInPoint.ViewModels;

public class SettingsViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public SettingsViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Настройки";
		_router = router;
	}
}