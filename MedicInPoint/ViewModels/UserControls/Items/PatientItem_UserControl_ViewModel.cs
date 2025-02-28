using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class PatientItem_UserControl_ViewModel : ViewModelBase
{
	[ObservableProperty]
	private Patient? _patient;

	[ObservableProperty]
	private bool _isSelected = false;
}