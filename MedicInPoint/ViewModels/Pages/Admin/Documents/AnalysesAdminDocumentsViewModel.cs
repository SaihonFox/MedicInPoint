using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.Views;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Mail;
using System.Text;

using Excel = Microsoft.Office.Interop.Excel;

namespace MedicInPoint.ViewModels.Pages.Admin.Documents;

public partial class AnalysesAdminDocumentsViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly INotificationService _notificationService = null!;
	private readonly IAppStateService _appService = null!;

	public AnalysesAdminDocumentsViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, INotificationService notificationService, IAppStateService appService) : this()
	{
		Title = "Отчеты результатов анализов";
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

	public int CurrentUserId => _appService.CurrentUser!.Id;

	async void FillAnalysisOrders()
	{
		_notificationService.Show("Уведомление", "Загрузка списка");
		using var response = _appService.CurrentUser!.Post!.Value == 1 ?
			await APIService.For<IAnalysisOrder>().GetAnalysisOrders().ConfigureAwait(false) :
			await APIService.For<IAnalysisOrder>().GetAnalysisOrders4User(_appService.CurrentUser.Id).ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		foreach(var analysisOrder in response.Content)
			Documents.Add(analysisOrder);
	}

	[RelayCommand]
	private async Task OutWord(AnalysisOrder order)
	{
		var app = new Excel.Application { Visible = true };
		var workBook = app.Workbooks.Add(Type.Missing);
		app.DisplayAlerts = false;
		var sheet = (Excel.Worksheet)app.Worksheets.Item[1];

		int maxNameLen = order.PatientAnalysisCart!.PatientAnalysisCartItems.Max(x => x.Analysis.Name.Length);
		int maxPriceLen = order.PatientAnalysisCart!.PatientAnalysisCartItems.Max(x => x.Analysis.Price.ToString("0.00").Length);

		List<string> list = [
			$"Запись на сдачу анализов на {order.AnalysisDatetime:dd.MM.yyyy HH:mm}",
			"< Анализы ",
			$"| на общую сумму {order.PatientAnalysisCart!.PatientAnalysisCartItems.ToList().Sum(x => x.Analysis.Price):0.00} руб.",
			"#",
		];
		if (!order.Comment.IsNullOrWhiteSpace())
			list.InsertRange(2, "", $"Комментарий пациента: {order.Comment}", "");
		list.AddRange(order.PatientAnalysisCart!.PatientAnalysisCartItems.Select(cartItem => $"|> {cartItem.Analysis.Name.PadLeft(Math.Abs(maxNameLen - cartItem.Analysis.Name.Length))} - {cartItem.Analysis.Price.ToString("0.00").PadRight(Math.Abs(cartItem.Analysis.Price.ToString("0.00").Length - maxPriceLen))} руб."));
		list.Add("< " + new string('#', 10));
		list.Add("");
		list.Add($"Лаборант: {order.User.FullName}");
		list.Add($"Пациент: {order.Patient.FullName}");
		for (int i = 1; i <= list.Count; i++)
			sheet.Range[$"A{i}"].Value = list[i - 1];
		sheet.Range[$"A1", $"A{list.Count}"].EntireColumn.AutoFit();
	}

	private readonly MailAddress from = new("saihonfox@yandex.ru", "Medic", Encoding.UTF8);
	private readonly SmtpClient client = new("smtp.yandex.ru", 25)
	{
		Credentials = new NetworkCredential(
			"saihonfox@yandex.ru",
			Environment.GetEnvironmentVariable("yandex_smtp_password", EnvironmentVariableTarget.Machine)!
		),
		EnableSsl = true,
	};

	[RelayCommand]
	private async Task SendEmail(AnalysisOrder order)
	{
		try
		{
			using var message = new MailMessage(from, new MailAddress(order.Patient.Email!));
			message.IsBodyHtml = false;
			message.BodyEncoding = Encoding.UTF8;
			message.Subject = "Запись на анализы";

			var sb = new StringBuilder();
			sb.AppendLine($"Вы успешно записаны на сдачу анализов на {order.AnalysisDatetime:dd.MM.yyyy HH:mm}");
			sb.AppendLine($"По адресу {order.Patient.Address}");
			sb.AppendLine();
			if (!string.IsNullOrWhiteSpace(order.Comment))
				sb.AppendLine($"Ваш комментарий: {order.Comment}");
			sb.AppendLine("< Анализы ");
			sb.AppendLine($"| на общую сумму {order.PatientAnalysisCart!.PatientAnalysisCartItems.ToList().Sum(x => x.Analysis.Price).ToString("0.00")} руб.");
			sb.AppendLine("#");

			int maxNameLen = order.PatientAnalysisCart!.PatientAnalysisCartItems.Max(x => x.Analysis.Name.Length);
			int maxPriceLen = order.PatientAnalysisCart!.PatientAnalysisCartItems.Max(x => x.Analysis.Price.ToString("0.00").Length);
			foreach (var cartItem in order.PatientAnalysisCart!.PatientAnalysisCartItems)
				sb.AppendLine($"|> {cartItem.Analysis.Name.PadLeft(Math.Abs(maxNameLen - cartItem.Analysis.Name.Length))} - {cartItem.Analysis.Price.ToString("0.00").PadRight(Math.Abs(cartItem.Analysis.Price.ToString("0.00").Length - maxPriceLen))} руб.");
			sb.AppendLine("< " + new string('#', 10));
			sb.AppendLine();

			sb.AppendLine($"Лаборант: {order.User!.FullName}");

			message.Body = sb.ToString();

			await client.SendMailAsync(message);
		} catch(Exception e)
		{
			_notificationService.Show("Err", $"Type: {e.GetType().FullName}, Message: {e.Message}");
		}
	}
}