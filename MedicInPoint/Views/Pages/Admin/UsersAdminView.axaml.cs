using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.UserControls.Drawers;
using MedicInPoint.ViewModels.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Admin;

public partial class UsersAdminView : UserControl
{
	private ObservableCollection<User> AllUsers = [];
	private ObservableCollection<UserItem_UserControl_View> AllUsersView = [];
	public UsersAdminViewModel ViewModel => (DataContext as UsersAdminViewModel)!;

	public UsersAdminView()
	{
		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		using var context = new LocalDbContext();
		var names = context.UsersAdminSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		drawer.DataContext = new UserDrawerViewModel();

		acb.KeyDown += acb_KeyDown;
		acb.TextChanged += acb_TextChanged;

		if (!Design.IsDesignMode)
			Loaded += (s, e) => FillUsers();

		App.services.GetRequiredService<MedicSignalRConnections>().UserConnection.On<User>("UserUpdated", user =>
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				var userView = AllUsersView.FirstOrDefault(x => x.ViewModel.User?.Id == user.Id);
				if (userView != null)
					userView.ViewModel.User = user;
				if(selectedUser != null)
					selectedUser.ViewModel.User = user;
				if(ViewModel.SelectedUser != null)
					ViewModel.SelectedUser = user;
			});
		});
	}

	private void acb_TextChanged(object? sender, TextChangedEventArgs e) => FillWithSearch();

	private UserItem_UserControl_View? selectedUser = null;

	void OnSelectUser(UserItem_UserControl_View view, bool isSelected)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			if (selectedUser != null)
			{
				selectedUser.ViewModel.IsSelected = false;
				selectedUser = null;
			}
			selectedUser = isSelected ? view : null;
			ViewModel.SelectedUser = selectedUser?.ViewModel.User;

			drawer.ViewModel.User = selectedUser?.ViewModel.User;
		});
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
		{
			var list = await context.UsersAdminSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			await context.UsersAdminSearches.AddAsync(new() { name = acb.SearchText });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}

	async void FillUsers()
	{
		using var response = await APIService.For<IUser>().GetUsers().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		AllUsers = [.. response.Content!];
		foreach (var user in AllUsers)
		{
			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				var view = new UserItem_UserControl_View
				{
					DataContext = new UserItem_UserControl_ViewModel
					{
						User = user,
						IsSelected = user.Id == selectedUser?.ViewModel.User?.Id
					},
					OnToggle = OnSelectUser
				};
				AllUsersView.Add(view);
				users_list.Items.Add(view);
			});
		}

		FillWithSearch();
	}

	async void FillWithSearch()
	{
		await Dispatcher.UIThread.InvokeAsync(users_list.Items.Clear);
		var patients = await Dispatcher.UIThread.InvokeAsync(() =>
			AllUsersView.Where(x =>
				x.ViewModel.User!.FullName.Contains(Dispatcher.UIThread.Invoke(() => acb.Text!), StringComparison.CurrentCultureIgnoreCase))
			.ToList()
		);

		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			if (patients.Find(x => x.ViewModel.User!.Id == selectedUser?.ViewModel.User?.Id) == null && selectedUser != null)
			{
				selectedUser.ViewModel.IsSelected = false;
				selectedUser = null;
			}
		});

		foreach (var patient in patients)
		{
			Dispatcher.UIThread.Invoke(() => {
				users_list.Items.Add(patient);
			});
		}

		await Dispatcher.UIThread.InvokeAsync(() => {
			centerText.IsVisible = AllUsers.Count == 0;
			centerText.Text = "Пустой список";
		});
		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			ViewModel.SelectedUser = selectedUser?.ViewModel.User;
			drawer.ViewModel.User = selectedUser?.ViewModel.User;
		});
	}
}