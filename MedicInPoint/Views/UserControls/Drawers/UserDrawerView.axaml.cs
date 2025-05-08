using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Drawers;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class UserDrawerView : UserControl
{
    public UserDrawerViewModel ViewModel => DataContext as UserDrawerViewModel;

    public UserDrawerView()
    {
        InitializeComponent();
    }
}