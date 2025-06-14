using Avalonia.Controls;

using MedicInPoint.Models;

namespace MedicInPoint.Views.UserControls.Items;

public partial class CategoryInAnalysisUserControl : UserControl
{
	public required AnalysisCategory Category { get; set; }

	public required Action<CategoryInAnalysisUserControl> OnDelete { get; set; }

	public CategoryInAnalysisUserControl()
	{
		InitializeComponent();

		if (Design.IsDesignMode)
			return;

		Loaded += (_, _) => name.Text = Category!.Name;

		delete_btn.Click += (_, _) => OnDelete?.Invoke(this);
	}
}