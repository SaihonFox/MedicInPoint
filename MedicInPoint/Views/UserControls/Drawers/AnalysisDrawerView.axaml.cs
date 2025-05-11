using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Drawers;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class AnalysisDrawerView : UserControl
{
    public AnalysisDrawerViewModel ViewModel => DataContext as AnalysisDrawerViewModel;

    public AnalysisDrawerView()
    {
        InitializeComponent();
    }
}