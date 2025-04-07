using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint.Views.UserControls.Items;

public partial class RnRRequest_UserControl_View : UserControl
{
    public RnRRequest_UserControl_ViewModel ViewModel => (dynamic)DataContext!;

	public RnRRequest_UserControl_View()
    {
        InitializeComponent();

        
    }
}