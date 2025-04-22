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
	public required Func<Request, Task<Request?>> OnAcceptRequest { get; set; }

	public required Action<Request> OnDeclineequest { get; set; }

	private readonly INotificationService? _notificationService = null;

	public RnRRequest_UserControl_ViewModel(INotificationService notificationService) : this()
	{
		_notificationService = notificationService;
	}

	[ObservableProperty]
	private Request? _request = null;

	[RelayCommand]
	private async Task AcceptRequest()
	{
		_notificationService?.Show("Запрос", "Одобрение");
		var request = await OnAcceptRequest?.Invoke(Request)!;
		if (request == null)
			return;
		_notificationService?.Show("Запрос", "Запрос одобрен");
		Request = request;
		/*Request.RequestStateId = 3;
		Request.RequestChanged = DateTime.Now;
		_notificationService.Show("Запрос", "Одобрение");
		File.WriteAllText(@"C:\Users\ILNAR\Desktop\r.json", JsonConvert.SerializeObject(Request, Formatting.Indented, new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			NullValueHandling = NullValueHandling.Include,
			Formatting = Formatting.Indented,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateFormatString = "dd'.'MM'.'yyyy' 'HH':'mm':'ss",
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
			Converters = [
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				new DateTimeConverter()
			]
		}));
		return;
		using var response = await APIService.For<IRequest>().UpdateRequest(Request);
		if (!response.IsSuccessStatusCode)
			return;

		_notificationService.Show("Запрос", "Запрос одобрен");
		Request = response.Content!;*/
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