using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint.Views.UserControls.Items;

public partial class Adder_UserControl_View : UserControl
{
	public Adder_UserControl_View()
	{
		InitializeComponent();
	}

	public Adder_UserControl_View(Adder_UserControl_ViewModel viewModel) : this()
	{
		DataContext = viewModel;
	}
}