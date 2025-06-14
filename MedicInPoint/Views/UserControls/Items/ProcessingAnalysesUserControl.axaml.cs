using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.Views.UserControls.Items;

public partial class ProcessingAnalysesUserControl : UserControl
{
	private INotificationService notification => App.services.GetRequiredService<INotificationService>();

	public required AnalysisOrder CurrentOrder { get; set; }

	public required Action<ProcessingAnalysesUserControl> OnApply { get; set; }

	public ProcessingAnalysesUserControl()
	{
		InitializeComponent();

		if (Design.IsDesignMode)
			return;

		DataContext = new List<string>(["Положительный", "Отрицательный"]);

		Loaded += (_, _) =>
		{
			apply.Click += Apply_Click;
			datetime.Text = CurrentOrder!.AnalysisDatetime.ToString("dd.MM.yyyy HH:mm");
			patient.Text = CurrentOrder?.Patient?.FullName;

			FillItems();
		};
	}

	void FillItems()
	{
		foreach(var item in CurrentOrder.PatientAnalysisCart!.PatientAnalysisCartItems)
			items_ic.Items.Add(new Border
			{
				DataContext = item,
				Background = Brushes.White,
				CornerRadius = new(15),
				Padding = new(8),
				Child = new Grid
				{
					RowSpacing = 4,
					RowDefinitions = new("auto,auto"),
				}.Also(x =>
				{
					var text = new SelectableTextBlock { Text = item.Analysis.Name, FontSize = 20 };
					Grid.SetRow(text, 0);
					x.Children.Add(text);

					var stackpanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 }.Also(x =>
					{
						x.Children.Add(new TextBlock { Text = "Результат", FontSize = 18, VerticalAlignment = VerticalAlignment.Center });
						x.Children.Add(new ComboBox { ItemsSource = new[] { "Положительный", "Отрицательный" }, SelectedIndex = 0, FontSize = 18, VerticalAlignment = VerticalAlignment.Center });
					});
					Grid.SetRow(stackpanel, 1);
					x.Children.Add(stackpanel);
				})
			});

	}

	async void Apply_Click(object? sender, RoutedEventArgs e)
	{
		using var orderResponse = await APIService.For<IAnalysisOrder>().Update(new AnalysisOrder
		{
			Id = CurrentOrder.Id,
			AnalysisDatetime = CurrentOrder.AnalysisDatetime,
			AtHome = CurrentOrder.AtHome,
			AnalysisOrderStateId = 2,
			Comment = CurrentOrder.Comment,
			PatientId = CurrentOrder.PatientId,
			UserId = CurrentOrder.UserId,
			PatientAnalysisCartId = CurrentOrder.PatientAnalysisCartId,
			RegistrationDate = CurrentOrder.RegistrationDate
		});
		if (!orderResponse.IsSuccessful)
		{
			notification.Show("Ошибка1!", $"{orderResponse.Error.Message}", NotificationType.Error);
			return;
		}


		foreach (var border in items_ic.Items.Cast<Border>().ToList())
		{
			var item = border.DataContext as PatientAnalysisCartItem;

			var combobox = ((border.Child as Grid)!.Children.First(x => x.GetType() == typeof(StackPanel)) as StackPanel)!.Children.First(x => x.GetType() == typeof(ComboBox)) as ComboBox;
			using var itemResponse = await APIService.For<IPatientAnalysisCartItem>().Update(new PatientAnalysisCartItem
			{
				Id = item.Id,
				PatientAnalysisCartId = item.PatientAnalysisCartId,
				AnalysisId = item.AnalysisId,
				ResultsDescription = combobox.SelectedValue.ToString()
			});
			if (!itemResponse.IsSuccessful)
			{
				notification.Show("Ошибка!", $"{orderResponse.Error!.Message}", NotificationType.Error);
				CurrentOrder.AnalysisOrderStateId = 1;
				await APIService.For<IAnalysisOrder>().Update(CurrentOrder);
				return;
			}
		}

		notification.Show("Успе!", "Вы успешно обновили анализ", NotificationType.Success);

		OnApply(this);
	}
}