using System.Collections.ObjectModel;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysisAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;

	public ViewModelBase DialogContent {
		get => App.services.GetService<FlyoutMenuViewModel>()!;
	}

	public ObservableCollection<Analysis> AnalysesList = [];
	
	public AnalysisAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Список анализов";
		_router = router;
	}

	[RelayCommand]
	public void Back() => _router.Back();
}