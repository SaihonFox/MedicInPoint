using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;

namespace MedicInPoint.ViewModels.Pages.Doctor.Documents;

public partial class AnalysesDoctorDocumentsViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	public AnalysesDoctorDocumentsViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Отчеты результатов анализов";
		_router = router;
	}

	[RelayCommand]
	private void Back() => _router.Back();
}