using Avalonia.Controls;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Converters.Json;
using MedicInPoint.Models;
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Items;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedicInPoint.Views.Pages.Doctor;

public partial class RnRDoctorView : UserControl
{
	public RnRDoctorView()
	{
		InitializeComponent();

		FillRequests();
	}

	async void FillRequests()
	{
		using var response = await APIService.For<IRequest>().GetRequests();
		if (!response.IsSuccessStatusCode)
			return;

		using var client = new HttpClient();
		try
		{
			await File.WriteAllTextAsync(@"C:\Users\ILNAR\Desktop\request.json", JsonConvert.SerializeObject(System.Text.Json.JsonSerializer.Deserialize<Request>(await client.GetStringAsync(@"https://medicapi.onrender.com/api/requests"), new System.Text.Json.JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
			})));
		}
		catch (System.Text.Json.JsonException ex)
		{
			await File.WriteAllTextAsync(@"C:\Users\ILNAR\Desktop\request.json", ex.Message + "\nStack Trace: " + ex.StackTrace + "\nLineNumber: " + ex.LineNumber + "\nPath: " + ex.Path);
		}
		catch (Exception ex)
		{
			
		}
		foreach (var request in response.Content!)
			requests_ic.Items.Add(new RnRRequest_UserControl_View {
				DataContext = new RnRRequest_UserControl_ViewModel {
					Request = request,
					OnAcceptRequest = RequestAccepted,
					OnDeclineequest = RequestDeclined
				}
			});
	}

	async void RequestAccepted(Request request)
	{
		request.RequestStateId = 3;
		request.RequestChanged = DateTime.Now;
		await APIService.For<IRequest>().UpdateRequest(request);
	}

	async void RequestDeclined(Request request)
	{
		request.RequestStateId = 2;
		request.RequestChanged = DateTime.Now;
		await APIService.For<IRequest>().UpdateRequest(request);
	}
}