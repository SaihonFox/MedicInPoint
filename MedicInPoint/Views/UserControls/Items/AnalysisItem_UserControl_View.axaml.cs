using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Lucdem.Avalonia.SourceGenerators.Attributes;

namespace MedicInPoint.Views.UserControls.Items;

public partial class AnalysisItem_UserControl_View : UserControl
{
    [AvaStyledProperty]
    private bool _isSelected = false;

    public AnalysisItem_UserControl_View()
    {
        InitializeComponent();
    }
}