using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class AnalysisCategoryItem_UserControl_ViewModel() : ViewModelBase
{
	[ObservableProperty]
	private AnalysisCategory? _analysisCategory;
}