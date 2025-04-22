using System.Collections.ObjectModel;

using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysisCategoriesAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;

	public ObservableCollection<AnalysisCategory> AnalysisCategoriesList = [];
	
	public AnalysisCategoriesAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Список категорий анализов";
		_router = router;
	}

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private string _searchText = string.Empty;

	async partial void OnSearchTextChanged(string value)
	{
		value = value.Trim();
		var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories();
		if (response.IsSuccessStatusCode)
			return;

		AnalysisCategoriesList = [.. response.Content!];
	}
}