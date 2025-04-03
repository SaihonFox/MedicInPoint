using System;

using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Svg;

using MedicInPoint.Models;
using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint.Views.UserControls.Items;

public partial class AnalysisCategoryItemAdminUserControl : UserControl
{
	public AnalysisCategoryItem_UserControl_ViewModel ViewModel = null!;

	public Action<AnalysisCategory?>? ActionOnSelect = null;

	public bool IsEditing { get; set; } = false;

	public AnalysisCategoryItemAdminUserControl()
	{
		ViewModel = (AnalysisCategoryItem_UserControl_ViewModel)DataContext!;

		InitializeComponent();

		edit_btn.Click += Edit_btn_Click;
		delete_btn.Click += Delete_btn_Click;
	}

	async void Edit_btn_Click(object? sender, RoutedEventArgs e)
	{
		/*if (!IsEditing)
		{
			ActionOnSelect?.Invoke(ViewModel!.Category);
			IsEditing = true;
			name.IsVisible = false;
			name_edit.IsVisible = true;
			name_edit.Focus();
			name_edit.SelectionStart = ViewModel!.Category!.Name.Length;
			name_edit.SelectionEnd = ViewModel!.Category!.Name.Length;
			(edit_btn.Content as Avalonia.Svg.Svg)!.Path = "/Assets/SVGs/buttons/confirm_edit.svg";
			(delete_btn.Content as Avalonia.Svg.Svg)!.Path = "/Assets/SVGs/buttons/reject_edit.svg";
		}
		else
		{
			if (string.IsNullOrWhiteSpace(name_edit.Text))
			{
				//new WindowNotificationManager(CategoriesWindow_Admin.Instance)
				//	.Show(new Notification("Внимание", "Пустые поля", NotificationType.Warning));
				return;
			}

			ActionOnSelect?.Invoke(null);
			IsEditing = false;
			name.IsVisible = true;
			name_edit.IsVisible = false;
			(edit_btn.Content as Avalonia.Svg.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
			(delete_btn.Content as Avalonia.Svg.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
			name.Text = name_edit.Text ?? ViewModel!.Category!.Name;
			try
			{
				//await APIService.For<IAnalysisCategory>().UpdateAnalysisCategory(ViewModel!.Category!.Id, ViewModel.Category);
			}
			catch (Exception ex)
			{
				new WindowNotificationManager(CategoriesWindow_Admin.Instance) { Position = NotificationPosition.BottomRight }
					.Show(new Notification("Ошибка", ex.StackTrace, NotificationType.Error));
			}
		}*/
	}

	async void Delete_btn_Click(object? sender, RoutedEventArgs e)
	{
		/*ActionOnSelect?.Invoke(null);
		name_edit.Text = name.Text ?? ViewModel!.Category!.Name;
		if (!IsEditing)
		{
			if (ViewModel!.Category!.Analyses.Count != 0)
			{
				new WindowNotificationManager(CategoriesWindow_Admin.Instance) { Position = NotificationPosition.BottomRight }
					.Show(new Notification("Ошибка", "Вы не можете удалить, т.к. она привязана", NotificationType.Error));
				return;
			}
			try
			{
				await APIService.For<IAnalysisCategory>().DeleteAnalysisCategory(ViewModel!.Category!.Id);
			}
			catch (Exception ex)
			{
				new WindowNotificationManager(CategoriesWindow_Admin.Instance)
					.Show(new Notification("Ошибка", ex.StackTrace, NotificationType.Error));
			}
		}
		else
		{
			IsEditing = false;
			name.IsVisible = true;
			name_edit.IsVisible = false;
			(edit_btn.Content as Avalonia.Svg.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
			(delete_btn.Content as Avalonia.Svg.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";
		}*/
	}

	public AnalysisCategoryItemAdminUserControl SetActionOnSelect(Action<AnalysisCategory?> action)
	{
		ActionOnSelect = action;
		return this;
	}

	public AnalysisCategoryItemAdminUserControl SetCategory(AnalysisCategory? category, int? selectedId = 0)
	{
		/*ViewModel!.Category = category;
		IsEditing = selectedId == category?.Id;*/
		return this;
	}

	public AnalysisCategoryItemAdminUserControl With(Action<AnalysisCategoryItemAdminUserControl> action)
	{
		action.Invoke(this);
		return this;
	}

	void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
	{

	}
}