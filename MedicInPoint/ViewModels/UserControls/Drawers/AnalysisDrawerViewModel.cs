using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Drawers;

public partial class AnalysisDrawerViewModel : ViewModelBase
{
	[ObservableProperty]
	private Analysis? _analysis;

	[ObservableProperty]
	private bool _isEditable = false;
}