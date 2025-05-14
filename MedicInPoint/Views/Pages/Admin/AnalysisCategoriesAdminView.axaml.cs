using System.Collections.ObjectModel;

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
using MedicInPoint.Views.UserControls.Drawers;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

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

				AnalysisDrawerView.AllCategories.Add(category);
			});
		});
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<AnalysisCategory>("AnalysisCategoryUpdated", category =>
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				var c = AllCategoriesView.First(x => x.ViewModel.AnalysisCategory!.Id == category.Id);
				c.ViewModel.AnalysisCategory = category;
				FillWithSearch();
			});
		});
	}

	void acb_TextChanged(object? sender, TextChangedEventArgs e)
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

	private AnalysisCategoryItemAdminUserControl? selectedCategory = null;

	async void FillCategories()
	{
		if (Design.IsDesignMode)
			return;

		App.services.GetRequiredService<INotificationService>().Show("Уведомление", "Загрузка списка");
		using var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

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
					ActionOnSelect = uc =>
					{
						if (selectedCategory != null)
						{
							selectedCategory.IsDeleting = false;
							selectedCategory.IsEditing = false;
							selectedCategory.name.IsVisible = true;
							selectedCategory.name_edit.IsVisible = false;
							(selectedCategory.edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
							(selectedCategory.delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
						}
						selectedCategory = uc;
					},
					ActionOnPreDelete = uc =>
					{
						if (selectedCategory != null)
						{
							selectedCategory.IsDeleting = false;
							selectedCategory.IsEditing = false;
							selectedCategory.name.IsVisible = true;
							selectedCategory.name_edit.IsVisible = false;
							(selectedCategory.edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
							(selectedCategory.delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
						}
						selectedCategory = uc;
					},
					ActionOnDelete = uc =>
					{
						AllCategories.Remove(uc.ViewModel.AnalysisCategory!);
						AllCategoriesView.Remove(uc);

						int index = categories_list.Items.IndexOf(uc);
						categories_list.Items.RemoveAt(index);
						categories_list.UpdateLayout();

						if (uc == selectedCategory)
							selectedCategory = null;

						var advCategory = AnalysisDrawerView.AllCategories.FirstOrDefault(x => x.Id == uc.ViewModel.AnalysisCategory!.Id);
						if(advCategory != null)
							AnalysisDrawerView.AllCategories.Remove(advCategory);
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
			centerText.IsVisible = categories.Count == 0;
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