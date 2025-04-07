using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class RnRRequest_UserControl_ViewModel : ViewModelBase
{
	public required Action OnAcceptRequest { get; set; }

	public required Action OnDeclineequest { get; set; }

	[ObservableProperty]
	private Request _request = new() { AnalysisDatetime = DateTime.Now, PatientAnalysisAddress = new() { Address = "там" } };

	[RelayCommand]
	private void AcceptRequest() => OnAcceptRequest?.Invoke();

	[RelayCommand]
	private void DeclineRequest() => OnDeclineequest.Invoke();
}