using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Drawers;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class AnalysisDrawerView : UserControl
{
    public static readonly AnalysisDrawerViewModel ViewModel = new();

    public AnalysisDrawerView()
    {
        DataContext = ViewModel;
        InitializeComponent();
    }
}