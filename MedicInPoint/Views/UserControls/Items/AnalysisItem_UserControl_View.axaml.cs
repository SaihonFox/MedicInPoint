using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using Lucdem.Avalonia.SourceGenerators.Attributes;

using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint.Views.UserControls.Items;

public partial class AnalysisItem_UserControl_View : UserControl
{
    public AnalysisItem_UserControl_ViewModel ViewModel => DataContext as AnalysisItem_UserControl_ViewModel;

    public AnalysisItem_UserControl_View()
    {
        InitializeComponent();
    }
}