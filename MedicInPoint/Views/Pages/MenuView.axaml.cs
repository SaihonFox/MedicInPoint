using Avalonia.Controls;

using MedicInPoint.ViewModels.Pages;

namespace MedicInPoint.Views.Pages;

public partial class MenuView : UserControl
{
	public readonly MenuViewModel ViewModel = new();

	public MenuView()
	{
		DataContext = ViewModel;
		InitializeComponent();

	}
}