using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

using MedicInPoint.Extensions;
using MedicInPoint.ViewModels.Pages.Admin;

using Microsoft.EntityFrameworkCore;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Admin;

public partial class AnalysesAdminView : UserControl
{
	public AnalysesAdminViewModel ViewModel => DataContext as AnalysesAdminViewModel;

	public AnalysesAdminView()
	{
		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		using var context = new LocalDbContext();
		var names = context.AnalysesAdminSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += acb_KeyDown;
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
		{
			var list = await context.AnalysesAdminSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			context.AnalysesAdminSearches.Add(new() { name = acb.SearchText });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}
}