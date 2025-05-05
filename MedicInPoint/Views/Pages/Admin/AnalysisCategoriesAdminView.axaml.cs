using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Admin;

public partial class AnalysisCategoriesAdminView : UserControl
{
	private List<AnalysisCategory> AllCategories = [];

	public AnalysisCategoriesAdminView()
	{
		InitializeComponent();
		
		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		using var context = new LocalDbContext();
		var names = context.AnalysisCategoriesAdminSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += acb_KeyDown;
		acb.TextChanged += acb_TextChanged;

		if(!Design.IsDesignMode)
			Loaded += (s, e) => FillCategories();

		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<AnalysisCategory>("AnalysisCategoryAdded", category =>
			Dispatcher.UIThread.Invoke(() => categories_list.Items.Add(new AnalysisCategoryItemAdminUserControl
			{
				DataContext = new AnalysisCategoryItem_UserControl_ViewModel
				{
					AnalysisCategory = category
				}
			}))
		);
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<AnalysisCategory>("AnalysisCategoryDeleted", category =>
		{
			var c = categories_list.Items.ToList().First(c => (c as AnalysisCategory)!.Id == category.Id);
			Dispatcher.UIThread.Invoke(() => categories_list.Items.Remove(c));
			centerText.IsVisible = categories_list.Items.Count == 0;
		});
	}

	private void acb_TextChanged(object? sender, TextChangedEventArgs e)
	{
		centerText.IsVisible = AllCategories.Count == 0;

		if (categories_list.Items.Cast<AnalysisCategoryItemAdminUserControl>().Any(x => x.IsEditing))
		{
			App.services.GetRequiredService<INotificationService>().Show("Ошибка", "Для начала отмените или подтвердите редактирование категории", NotificationType.Error);
			return;
		}

		categories_list.Items.Clear();

		var searchList = acb.Text.IsNullOrWhiteSpace() ? AllCategories : AllCategories.Where(x => x.Name.Contains(acb.Text!, StringComparison.CurrentCultureIgnoreCase)).ToList();
		centerText.IsVisible = searchList.Count == 0;
		foreach (var category in searchList)
		{
			categories_list.Items.Add(new AnalysisCategoryItemAdminUserControl
			{
				DataContext = new AnalysisCategoryItem_UserControl_ViewModel
				{
					AnalysisCategory = category
				}
			});
		}
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
		{
			var list = await context.AnalysisCategoriesAdminSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			await context.AnalysisCategoriesAdminSearches.AddAsync(new() { name = acb.SearchText });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}

	async void FillCategories()
	{
		categories_list.Items.Clear();
		if (Design.IsDesignMode)
			return;
		using var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;
		AllCategories = [..response.Content];
		var categories = Dispatcher.UIThread.Invoke(() => AllCategories.Where(x => x.Name.Contains(acb.Text!, StringComparison.CurrentCultureIgnoreCase)).ToList());
		Dispatcher.UIThread.Invoke(() => {
			centerText.IsVisible = AllCategories.Count == 0;
			centerText.Text = "Пустой список";
		});
		foreach (var category in categories)
		{
			Dispatcher.UIThread.Invoke(() => {
				categories_list.Items.Add(new AnalysisCategoryItemAdminUserControl
				{
					DataContext = new AnalysisCategoryItem_UserControl_ViewModel
					{
						AnalysisCategory = category
					}
				});
			});
		}
	}
}