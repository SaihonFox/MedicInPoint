using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Drawers;

public partial class PatientDrawerViewModel : ViewModelBase
{
	[ObservableProperty]
	private Patient? _patient;
}