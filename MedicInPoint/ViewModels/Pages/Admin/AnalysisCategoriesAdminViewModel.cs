using System.Collections.ObjectModel;

using Avalonia.Controls.Notifications;
using Avalonia.SimpleRouter;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class AnalysisCategoriesAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	public readonly INotificationService _notificationService = null!;

	public ObservableCollection<AnalysisCategory> AnalysisCategoriesList = [];
	
	public AnalysisCategoriesAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, INotificationService notificationService) : this()
	{
		Title = "Список категорий анализов";
		_router = router;
		_notificationService = notificationService;
	}

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private AnalysisCategory _analysisCategory = new();

	[RelayCommand]
	private async Task AddCategory()
	{
		if (AnalysisCategory.Name.IsNullOrWhiteSpace())
		{
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("Ошибка", "Введите имя категории", NotificationType.Error));
			return;
		}

		using var response = await APIService.For<IAnalysisCategory>().AddAnalysisCategory(AnalysisCategory);
		if (!response.IsSuccessful)
		{
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("Ошибка!", "Ошибка добавления категории"));
			return;
		}

		Dispatcher.UIThread.Invoke(() => _notificationService.Show("Успех!", "Категория успешно добавлена!"));
		AnalysisCategory = new();
	}
}