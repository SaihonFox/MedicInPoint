using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

using MedicInPoint.Extensions;

using Microsoft.EntityFrameworkCore;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Admin;

public partial class AnalysisAdminView : UserControl
{
	private const string SEARCH_FILE = @"analysis_admin-search.txt";

	public AnalysisAdminView()
	{
		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		var context = LocalStorage.context;

		var names = context.AnalysisAdminSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += async (_, e) =>
		{
			if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
			{
				var list = await context.AnalysisAdminSearches.ToListAsync();
				if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
					return;

				context.AnalysisAdminSearches.Add(new() { name = acb.SearchText });
				await context.SaveChangesAsync();

				var enumerable = acb.ItemsSource.Cast<string>().ToList();
				enumerable.Add(acb.SearchText);
				enumerable = [.. enumerable.Distinct()];
				acb.ItemsSource = enumerable;
			}
		};
	}
}