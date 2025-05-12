using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint.Views.UserControls.Items;

public partial class AnalysisItem_UserControl_View : UserControl
{
    public AnalysisItem_UserControl_ViewModel ViewModel => DataContext as AnalysisItem_UserControl_ViewModel;

	public Action<AnalysisItem_UserControl_View, bool> OnToggle { get; set; } = null!;

	public AnalysisItem_UserControl_View()
    {
        InitializeComponent();

		select_tb.IsCheckedChanged += (_, _) =>
		{
			OnToggle?.Invoke(this, select_tb.IsChecked!.Value);
		};
	}
}