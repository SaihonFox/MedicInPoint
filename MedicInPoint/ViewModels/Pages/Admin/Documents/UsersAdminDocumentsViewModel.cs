using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Logging;
using Avalonia.SimpleRouter;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Models;
using MedicInPoint.Services;

using System.Collections.ObjectModel;
using System.Net.Sockets;

using Excel = Microsoft.Office.Interop.Excel;

namespace MedicInPoint.ViewModels.Pages.Admin.Documents;

public partial class UsersAdminDocumentsViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly INotificationService _notificationService = null!;
	private readonly IAppStateService _appService = null!;

	public UsersAdminDocumentsViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, INotificationService notificationService, IAppStateService appService) : this()
	{
		Title = "Отчеты лаборантов";
		_router = router;
		_notificationService = notificationService;
		_appService = appService;

		FillAnalysisOrders();
		FillUsers();
	}

	[RelayCommand]
	private void Back() => _router.Back();

	public string CurrentUser => _appService.CurrentUser!.FullName;

	[ObservableProperty]
	private ObservableCollection<User> _users = [];

	[ObservableProperty]
	private ObservableCollection<AnalysisOrder> _allDocuments = [];
	[ObservableProperty]
	private ObservableCollection<AnalysisOrder> _documents = [];

	[ObservableProperty]
	private User? _selectedUser = null;

	[ObservableProperty]
	private int? _selectedUserIndex = Design.IsDesignMode ? 0 : null;

	async void FillAnalysisOrders()
	{
		_notificationService.Show("Уведомление", "Загрузка списка");
		using var response = _appService.CurrentUser!.Post!.Value == 1 ?
			await APIService.For<IAnalysisOrder>().GetAnalysisOrders().ConfigureAwait(false) :
			await APIService.For<IAnalysisOrder>().GetAnalysisOrders4User(_appService.CurrentUser.Id).ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		foreach (var analysisOrder in response.Content)
			AllDocuments.Add(analysisOrder);
	}

	async Task FillUsers()
	{
		try
		{
			_notificationService.Show("Уведомление", "Загрузка списка сотрудников");
			using var response = await APIService.For<IUser>().GetUsers().ConfigureAwait(false);
			if (!response.IsSuccessful)
				return;

			Users = [.. response.Content.Where(x => x.Post == 2)];
		}
		catch (HttpRequestException ex) when (ex.GetBaseException() is SocketException sex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "SHttpError", this, $"|Patients|Message: {sex.Message}\nStatus code: {sex.ErrorCode}, RequestError: {sex.SocketErrorCode}");
			if (sex.SocketErrorCode == SocketError.ConnectionReset)
				Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", "Подключение прервано", NotificationType.Error));
		}
		catch (HttpRequestException ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "HttpError", this, $"|Patients|Message: {ex.Message}\nStatus code: {ex.StatusCode}, RequestError: {ex.HttpRequestError}");
			Dispatcher.UIThread.Invoke(() => _notificationService.Show("Уведомление!", ex.Message, NotificationType.Error));
		}
		catch (Exception ex)
		{
			Logger.Sink!.Log(LogEventLevel.Error, "Error", this, $"{ex.GetBaseException().GetType().FullName}-{ex.HResult}|Message: {ex.Message}\nsource: {ex.GetType().FullName}");
		}
	}

	partial void OnSelectedUserChanged(User? value)
	{
		if (value == null)
			return;

		Documents = [.. AllDocuments.Where(x => x.UserId == value.Id)];
	}

	[RelayCommand]
	private void BeautyExcelReport(AnalysisOrder order)
	{
		var app = new Excel.Application { Visible = true };
		var workBook = app.Workbooks.Add(Type.Missing);
		app.DisplayAlerts = false;
		var sheet = (Excel.Worksheet)app.Worksheets.Item[1];

		sheet.Range["A1"].Value = $"Результаты анализов на {order.AnalysisDatetime:dd.MM.yyyy HH:mm}";
		sheet.Range["A3"].Value = "Анализы";
		sheet.Range["A3"].Font.Bold = true;
		sheet.Range["A4"].Value = "Наименование анализа";
		sheet.Range["B4"].Value = "Результат анализа";
		sheet.Range["A4"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
		sheet.Range["B4"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
		sheet.Range["A4"].Font.Bold = true;
		sheet.Range["B4"].Font.Bold = true;

		int analysisCount = order.PatientAnalysisCart!.PatientAnalysisCartItems.Count;
		for (int i = 0; i < analysisCount; i++)
		{
			var cartItem = order.PatientAnalysisCart.PatientAnalysisCartItems.ToList()[i];
			sheet.Range[$"A{5 + i}"].Value = cartItem.Analysis.Name;
			sheet.Range[$"B{5 + i}"].Value = cartItem.ResultsDescription;
		}
		sheet.Range[sheet.Range["A4"], sheet.Range[$"B{4 + analysisCount}"]].Borders.Color = Excel.XlRgbColor.rgbBlack;
		sheet.Range[sheet.Range["A4"], sheet.Range[$"B{4 + analysisCount}"]].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


		sheet.Range[$"A{6 + analysisCount}"].Value = $"ФИО Лаборанта: {order.User.FullName}";
		sheet.Range[$"A{7 + analysisCount}"].Value = $"ФИО Клиента: {order.Patient.FullName}";
		sheet.Range[$"A{8 + analysisCount}"].Value = $"Адрес забора анализов: {(order.AtHome ? order.Patient.Address : "В клинике")}";
		if (order.Comment != null)
			sheet.Range[$"A{9 + analysisCount}"].Value = $"Комментарии клиента: {order.Comment}";

		sheet.Rows.EntireRow.AutoFit();
		sheet.Columns.EntireColumn.AutoFit();
	}
}