using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Extensions;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.UserControls.Items;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.Views.UserControls.Items;

public partial class AnalysisCategoryItemAdminUserControl : UserControl
{
	public AnalysisCategoryItem_UserControl_ViewModel ViewModel => (DataContext as AnalysisCategoryItem_UserControl_ViewModel)!;

	public readonly INotificationService notification = null!;

	public Action<AnalysisCategoryItemAdminUserControl>? ActionOnSelect = null;
	public Action<AnalysisCategoryItemAdminUserControl>? ActionOnDelete = null;
	public Action<AnalysisCategoryItemAdminUserControl>? ActionOnPreDelete = null;

	public bool IsEditing { get; set; } = false;
	public bool IsDeleting { get; set; } = false;
	private string initName = string.Empty;

	public AnalysisCategoryItemAdminUserControl()
	{
		notification = App.services.GetRequiredService<INotificationService>();

		InitializeComponent();

		edit_btn.Click += Edit_btn_Click;
		delete_btn.Click += Delete_btn_Click;

		if(!Design.IsDesignMode)
			Loaded += (_, _) => Dispatcher.UIThread.Invoke(() => initName = (DataContext as AnalysisCategoryItem_UserControl_ViewModel)!.AnalysisCategory!.Name);
	}

	async void Edit_btn_Click(object? sender, RoutedEventArgs e)
	{
		if (!IsEditing)
		{
			if (IsDeleting)
			{
				if (ViewModel!.AnalysisCategoriesList.Count != 0)
				{
					notification.Show("Ошибка", "Вы не можете удалить, т.к. она привязана", NotificationType.Error);
					return;
				}
				try
				{
					using var response = await APIService.For<IAnalysisCategory>().DeleteAnalysisCategory(ViewModel!.AnalysisCategory!.Id);
					if (!response.IsSuccessful)
						notification.Show("Ошибка!", $"Не удалось удалить: {response.StatusCode}", NotificationType.Error);
					else
					{
						notification.Show("Успех!", "Успешно удалено", NotificationType.Success);
						IsDeleting = false;
						(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
						(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";

						ActionOnDelete?.Invoke(this);
					}
				}
				catch (Exception ex)
				{
					notification.Show("Ошибка", ex.StackTrace!, NotificationType.Error);
				}
			}
			else
			{
				ActionOnSelect?.Invoke(this);
				IsEditing = true;
				name.IsVisible = false;
				name_edit.IsVisible = true;
				name_edit.Focus();
				name_edit.SelectionStart = ViewModel!.AnalysisCategory!.Name.Length;
				name_edit.SelectionEnd = ViewModel!.AnalysisCategory!.Name.Length;
				(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/confirm_edit.svg";
				(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/reject_edit.svg";
			}
		}
		else
		{
			if (name_edit.Text.IsNullOrWhiteSpace())
			{
				notification.Show("Внимание", "Пустые поля", NotificationType.Warning);
				return;
			}

			if (initName.Equals(ViewModel.AnalysisCategory!.Name))
			{
				IsEditing = false;
				name.IsVisible = true;
				name_edit.IsVisible = false;
				(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
				(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
				name.Text = name_edit.Text ?? ViewModel!.AnalysisCategory!.Name;
				return;
			}

			ActionOnSelect?.Invoke(null);
			
			try
			{
				using var response = await APIService.For<IAnalysisCategory>().UpdateAnalysisCategory(ViewModel!.AnalysisCategory!);
				if (!response.IsSuccessful)
					notification.Show("Ошибка!", $"Не удалось редактировать: {response.StatusCode}", NotificationType.Error);
				else
				{
					IsEditing = false;
					name.IsVisible = true;
					name_edit.IsVisible = false;
					(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
					(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
					name.Text = name_edit.Text ?? ViewModel!.AnalysisCategory!.Name;
					notification.Show("Успех!", "Успешно отредактировано", NotificationType.Success);
					initName = ViewModel.AnalysisCategory!.Name;
				}
			}
			catch (Exception ex)
			{
				notification.Show("Ошибка", ex.StackTrace!, NotificationType.Error);
			}
		}
	}

	async void Delete_btn_Click(object? sender, RoutedEventArgs e)
	{
		ActionOnSelect?.Invoke(null);
		name_edit.Text = name.Text ?? ViewModel!.AnalysisCategory!.Name;
		if (!IsEditing)
		{
			if (!IsDeleting)
			{
				ActionOnPreDelete?.Invoke(this);
				IsDeleting = true;
				(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/confirm_edit.svg";
				(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/reject_edit.svg";
			}
			else
			{
				IsDeleting = false;
				(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
				(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
			}
		}
		else
		{
			IsEditing = false;
			name.IsVisible = true;
			name_edit.IsVisible = false;
			(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
			(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
		}
	}
}