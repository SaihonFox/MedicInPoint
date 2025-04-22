using Avalonia.Controls;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Models;
using MedicInPoint.ViewModels.Pages.Doctor;
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Items;

namespace MedicInPoint.Views.Pages.Doctor;

public partial class RnRDoctorView : UserControl
{
	private readonly RnRDoctorViewModel ViewModel = null!;

	public RnRDoctorView()
	{
		ViewModel = (RnRDoctorViewModel)DataContext!;

		InitializeComponent();

		if(!Design.IsDesignMode)
			FillRequests();
	}

	async void FillRequests()
	{
		using var response = await APIService.For<IRequest>().GetRequests();
		if (!response.IsSuccessful)
			return;

		foreach (var request in response.Content!)
		{
			//File.WriteAllText(@$"C:\Users\ILNAR\Desktop\r-{request?.Id}.txt", (request == null).ToString());
			requests_ic.Items.Add(new RnRRequest_UserControl_View
			{
				DataContext = new RnRRequest_UserControl_ViewModel
				{
					Request = request,
					OnAcceptRequest = RequestAccepted,
					OnDeclineequest = RequestDeclined
				}
			});
		}
	}

	async Task<Request?> RequestAccepted(Request request)
	{
		request.RequestStateId = 3;
		request.RequestChanged = DateTime.Now;
		using var response = await APIService.For<IRequest>().UpdateRequest(request);
		await File.WriteAllTextAsync(@$"C:\Users\ILNAR\Desktop\r-n.txt", response.IsSuccessful.ToString() + "\n");
		if (!response.IsSuccessful)
			return null;
		await File.AppendAllTextAsync(@$"C:\Users\ILNAR\Desktop\r-n.txt", response.StatusCode.ToString());
		return response.Content!;
	}

	async void RequestDeclined(Request request)
	{
		request.RequestStateId = 2;
		request.RequestChanged = DateTime.Now;
		await APIService.For<IRequest>().UpdateRequest(request);
	}
}