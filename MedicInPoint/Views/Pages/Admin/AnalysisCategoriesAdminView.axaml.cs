using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

using MedicInPoint.API.AIMLAPI.Models;
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

using Newtonsoft.Json;

namespace MedicInPoint.Views.Pages.Admin;

public partial class AnalysisCategoriesAdminView : UserControl
{
	private ObservableCollection<AnalysisCategory> AllCategories = [];
	private ObservableCollection<AnalysisCategoryItemAdminUserControl> AllCategoriesView = [];

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
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				var categoryView = new AnalysisCategoryItemAdminUserControl
				{
					DataContext = new AnalysisCategoryItem_UserControl_ViewModel
					{
						AnalysisCategory = category,
						AnalysisCategoriesList = category.AnalysisCategoriesLists
					}
				};
				categories_list.Items.Add(categoryView);
				AllCategories.Add(category);
				AllCategoriesView.Add(categoryView);
				FillWithSearch();
			});
		});
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<AnalysisCategory>("AnalysisCategoryUpdated", category =>
		{
			FillWithSearch();
		});
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<AnalysisCategory>("AnalysisCategoryDeleted", category =>
		{
			/*var c = AllCategoriesView.First(x => x.ViewModel.AnalysisCategory!.Id == category.Id);
			int index = AllCategoriesView.IndexOf(c);
			categories_list.Items.RemoveAt(index);
			AllCategories.Remove(c.ViewModel.AnalysisCategory!);
			AllCategoriesView.Remove(c);
			FillWithSearch();*/
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

		FillWithSearch();
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
		if (Design.IsDesignMode)
			return;
		using var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		await File.WriteAllTextAsync(@"C:\Users\ILNAR\Desktop\content.txt", JsonConvert.SerializeObject(response.Content, Formatting.Indented));
		AllCategories = [..response.Content];
		foreach(var category in AllCategories) {
			Dispatcher.UIThread.Invoke(() => {
				var categoryView = new AnalysisCategoryItemAdminUserControl
				{
					DataContext = new AnalysisCategoryItem_UserControl_ViewModel
					{
						AnalysisCategory = category,
						AnalysisCategoriesList = category.AnalysisCategoriesLists,
					},
					ActionOnDelete = uc => {
						categories_list.Items.Remove(uc);
						categories_list.UpdateLayout();
						AllCategoriesView.Remove(uc);
						AllCategories.Remove(uc.ViewModel.AnalysisCategory!);
					}
				};
				categories_list.Items.Add(categoryView);
				AllCategoriesView.Add(categoryView);
			});
		}

		FillWithSearch();
	}

	async void FillWithSearch()
	{
		categories_list.Items.Clear();
		var categories = await Dispatcher.UIThread.InvokeAsync(() => AllCategoriesView.Where(x => x.ViewModel.AnalysisCategory!.Name.Contains(acb.Text!, StringComparison.CurrentCultureIgnoreCase)).ToList());
		Dispatcher.UIThread.Invoke(() => {
			centerText.IsVisible = AllCategories.Count == 0;
			centerText.Text = "Пустой список";
		});
		foreach (var category in categories)
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				categories_list.Items.Add(category);
			});
		}
	}
}