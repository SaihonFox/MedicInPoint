using System.Collections.ObjectModel;

using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.Models;
using MedicInPoint.Services;

namespace MedicInPoint.ViewModels.Pages.Admin.Documents;

public partial class PatientsAdminDocumentsViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly INotificationService _notificationService = null!;
	private readonly IAppStateService _appService = null!;

	public PatientsAdminDocumentsViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, INotificationService notificationService, IAppStateService appService) : this()
	{
		Title = "Отчеты пациентов";
		_router = router;
		_notificationService = notificationService;
		_appService = appService;

		FillAnalysisOrders();
	}

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private ObservableCollection<AnalysisOrder> _documents = [];

	public string CurrentUser => _appService.CurrentUser!.FullName;

	async void FillAnalysisOrders()
	{
		_notificationService.Show("Уведомление", "Загрузка списка");
		using var response = _appService.CurrentUser!.Post!.Value == 1 ?
			await APIService.For<IAnalysisOrder>().GetAnalysisOrders().ConfigureAwait(false) :
			await APIService.For<IAnalysisOrder>().GetAnalysisOrders4User(_appService.CurrentUser.Id).ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		foreach (var analysisOrder in response.Content)
			Documents.Add(analysisOrder);
	}

	[RelayCommand]
	private void OutDocument(AnalysisOrder analysisOrder)
	{

	}
}