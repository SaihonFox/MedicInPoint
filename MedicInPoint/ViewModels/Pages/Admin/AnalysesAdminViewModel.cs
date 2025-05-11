using System.Collections.ObjectModel;

using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Models;

using Microsoft.AspNetCore.SignalR.Client;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysesAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	[ObservableProperty]
	private ObservableCollection<Analysis> _allAnalyses = [];

	[ObservableProperty]
	private ObservableCollection<Analysis> _analysesList = [];
	
	public AnalysesAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, MedicSignalRConnections connections) : this()
	{
		Title = "Список анализов";
		_router = router;

		FillAnalyses();
		
		connections.AnalysisConnection.On<Analysis>("AnalysisAdded", AllAnalyses.Add);
	}

	async void FillAnalyses()
	{
		var response = await APIService.For<IAnalysis>().GetAnalyses();
		if (!response.IsSuccessStatusCode)
			return;

		AllAnalyses = [.. response.Content!];
		//SelectedPatientIndex = 0;
		Log("none", "PatientDoctor");
		AnalysesList = SearchAnalysis();
	}

	ObservableCollection<Analysis> SearchAnalysis() => [.. AllAnalyses.Where(a =>
		new[] {
			a.Name, $"{a.Price}", a.ResultsAfter, a.Biomaterial, a.Description, a.Preparation
		}.Any(s => s?.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ?? false) )
	];

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private string _searchText = string.Empty;

	partial void OnAllAnalysesChanged(ObservableCollection<Analysis> value)
	{
		AnalysesList = SearchAnalysis();
	}

	async partial void OnSearchTextChanged(string value)
	{
		value = value.Trim();
		var response = await APIService.For<IAnalysis>().GetAnalyses();
		if (response.IsSuccessStatusCode)
			return;

		AnalysesList = [.. response.Content!];
	}

	[ObservableProperty]
	private bool _isSelected = false;

	[ObservableProperty]
	private Analysis? _selectedAnalysis = null;//Design.IsDesignMode ? null : new() { Name = "Какое то имя", Price = 169, Biomaterial = "Венозная кровь", ResultsAfter = "2 дня", Preparation = "Подготовка к анализу" };
}