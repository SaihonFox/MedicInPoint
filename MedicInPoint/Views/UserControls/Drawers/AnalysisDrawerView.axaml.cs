using Avalonia.Controls;
using Avalonia.Interactivity;

using MedicInPoint.ViewModels.UserControls.Drawers;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class AnalysisDrawerView : UserControl
{
	public AnalysisDrawerViewModel ViewModel => DataContext as AnalysisDrawerViewModel;

	public AnalysisDrawerView()
	{
		InitializeComponent();

		edit_btn.Click += first_btn_Click;
		delete_btn.Click += second_btn_Click;
	}

	void second_btn_Click(object? sender, RoutedEventArgs e)
	{
		
	}

	void first_btn_Click(object? sender, RoutedEventArgs e)
	{
		
	}
}