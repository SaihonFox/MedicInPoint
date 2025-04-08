using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Converters.Json;
using MedicInPoint.Models;
using MedicInPoint.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class RnRRequest_UserControl_ViewModel() : ViewModelBase
{
	public required Action<Request> OnAcceptRequest { get; set; }

	public required Action<Request> OnDeclineequest { get; set; }

	private readonly INotificationService _notificationService = null!;

	public RnRRequest_UserControl_ViewModel(INotificationService notificationService) : this()
	{
		_notificationService = notificationService;
	}

	[ObservableProperty]
	private Request _request = null!;

	[RelayCommand]
	private async Task AcceptRequest()
	{
		//OnAcceptRequest?.Invoke(Request);
		Request.RequestStateId = 3;
		Request.RequestChanged = DateTime.Now;
		_notificationService.Show("Запрос", "Одобрение");
		File.WriteAllText(@"C:\Users\ILNAR\Desktop\r.json", JsonConvert.SerializeObject(Request, Formatting.Indented, new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			NullValueHandling = NullValueHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateFormatString = "dd'-'MM'-'yyyy'T'HH':'mm':'ss.FFFFFFF",
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
			Converters = [
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				new DateTimeConverter()
			],
		}));
		return;
		using var response = await APIService.For<IRequest>().UpdateRequest(Request);
		if (!response.IsSuccessStatusCode)
			return;

		_notificationService.Show("Запрос", "Запрос одобрен");
		Request = response.Content!;
	}

	[RelayCommand]
	private async Task DeclineRequest()
	{
		//OnDeclineequest?.Invoke(Request);
		Request.RequestStateId = 2;
		Request.RequestChanged = DateTime.Now;
		_notificationService.Show("Запрос", "Отклонение");
		using var response = await APIService.For<IRequest>().UpdateRequest(Request);
		if (!response.IsSuccessStatusCode)
			return;

		_notificationService.Show("Запрос", "Запрос отклонен");
		Request = response.Content!;
	}
}