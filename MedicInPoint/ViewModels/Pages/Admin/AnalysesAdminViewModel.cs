using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysesAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	
	public AnalysesAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Список анализов";
		_router = router;
	}

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private string _searchText = string.Empty;

	[ObservableProperty]
	private Analysis? _selectedAnalysis = null;//Design.IsDesignMode ? null : new() { Name = "Какое то имя", Price = 169, Biomaterial = "Венозная кровь", ResultsAfter = "2 дня", Preparation = "Подготовка к анализу" };
}