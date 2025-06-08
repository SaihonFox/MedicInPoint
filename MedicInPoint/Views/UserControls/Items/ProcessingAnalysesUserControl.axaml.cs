using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;

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
		if(items_ic.Items.Cast<PatientAnalysisCartItem>().Any(x => x.ResultsDescription.IsNullOrWhiteSpace()))
		{
			App.services.GetRequiredService<INotificationService>().Show("Ошибка!", "Вы не заполнили все поля с результатом");
			return;
		}

		CurrentOrder.AnalysisOrderStateId = 2;

		using var orderResponse = await APIService.For<IAnalysisOrder>().Update(CurrentOrder);
		if (!orderResponse.IsSuccessful)
		{
			notification.Show("Ошибка1!", $"{orderResponse.Error.Message}", NotificationType.Error);
			return;
		}

		foreach(var item in items_ic.Items.Cast<PatientAnalysisCartItem>().ToList())
		{
			using var itemResponse = await APIService.For<IPatientAnalysisCartItem>().Update(item);
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