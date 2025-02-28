using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Drawers;

namespace MedicInPoint;

public partial class PatientDrawerView : UserControl
{
    public static PatientDrawerViewModel ViewModel = new();

    public PatientDrawerView()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }
}