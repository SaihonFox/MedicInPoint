using System.Collections.ObjectModel;

using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysisCategoriesAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;

	public ObservableCollection<Analysis> AnalysesList = [];
	
	public AnalysisCategoriesAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Список категорий анализов";
		_router = router;
	}

	[RelayCommand]
	public void Back() => _router.Back();
}