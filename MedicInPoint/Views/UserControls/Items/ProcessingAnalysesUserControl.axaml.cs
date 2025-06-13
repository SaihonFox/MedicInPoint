using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

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
			items_ic.Items.Add(item);

	}

	async void Apply_Click(object? sender, RoutedEventArgs e)
	{
		foreach (var item in items_ic.Items)
		{
			var grid = ((items_ic.ContainerFromItem(item) as ContentPresenter).Child as Border).Child as Grid;
			notification.Show("temp",
			(
				(
					(grid.Children.First(x => x.GetType() == typeof(StackPanel)) as StackPanel).Children.First(x => x.GetType() == typeof(ComboBox)) as ComboBox
				).SelectedValue
			).ToString()
		);
		}
		/*var grid = (items_ic.ItemTemplate.Build(null) as Border).Child as Grid;
		notification.Show("temp",
			(
				(
					(grid.Children.First(x => x.GetType() == typeof(StackPanel)) as StackPanel).Children.First(x => x.GetType() == typeof(ComboBox)) as ComboBox
				)
			).ToString()
		);*/
		return;

		if(items_ic.Items.Cast<PatientAnalysisCartItem>().Any(x => x.ResultsDescription.IsNullOrWhiteSpace()))
		{
			App.services.GetRequiredService<INotificationService>().Show("Ошибка!", "Вы не заполнили все поля с результатом");
			return;
		}

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

		foreach(var item in items_ic.Items.Cast<PatientAnalysisCartItem>().ToList())
		{
			using var itemResponse = await APIService.For<IPatientAnalysisCartItem>().Update(new PatientAnalysisCartItem { Id = item.Id, PatientAnalysisCartId = item.PatientAnalysisCartId, AnalysisId = item.AnalysisId, ResultsDescription = item.ResultsDescription });
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