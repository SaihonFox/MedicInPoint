using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Drawers;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class UserDrawerView : UserControl
{
    public static UserDrawerViewModel ViewModel = new();

    public UserDrawerView()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }
}