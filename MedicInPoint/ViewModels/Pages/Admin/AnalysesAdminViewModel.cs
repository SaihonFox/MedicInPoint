using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;
using MedicInPoint.Services;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysesAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly IAppStateService _appService = null!;

	public AnalysesAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, IAppStateService appService) : this()
	{
		Title = "Список анализов";
		_router = router;
		_appService = appService;
	}

	[RelayCommand]
	private void Back() => _router.Back();

	public string CurrentUser => _appService.CurrentUser!.FullName;

	[ObservableProperty]
	private string _searchText = string.Empty;

	[ObservableProperty]
	private Analysis? _selectedAnalysis = null;//Design.IsDesignMode ? null : new() { Name = "Какое то имя", Price = 169, Biomaterial = "Венозная кровь", ResultsAfter = "2 дня", Preparation = "Подготовка к анализу" };
}