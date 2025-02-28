using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class AnalysisItem_UserControl_ViewModel : ViewModelBase
{
	[ObservableProperty]
	private Analysis? _analysis;
}