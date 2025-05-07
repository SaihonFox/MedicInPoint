using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint.Views.UserControls.Items;

public partial class PatientItem_UserControl_View : UserControl
{
    public PatientItem_UserControl_ViewModel ViewModel => DataContext as PatientItem_UserControl_ViewModel;

    public Action<PatientItem_UserControl_View, bool> OnToggle { get; set; } = null!;

	public PatientItem_UserControl_View()
    {
        InitializeComponent();
        
        select_tb.IsCheckedChanged += (_, _) =>
        {
            OnToggle?.Invoke(this, select_tb.IsChecked!.Value);
        };
    }
}