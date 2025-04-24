using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Medic.Theme.Controls.Custom;

public class DateTimePicker : PickerPresenterBase
{
	public DateTimePicker() : base()
	{
		Value = DateTime.Now;
	}

	public static readonly DirectProperty<DateTimePicker, DateTime> DateOnlyProperty =
		AvaloniaProperty.RegisterDirect<DateTimePicker, DateTime>(nameof(DateOnly),
			x => x.DateOnly, (x, v) => x.DateOnly = v);

	public static readonly DirectProperty<DateTimePicker, int> HourProperty =
		AvaloniaProperty.RegisterDirect<DateTimePicker, int>(nameof(Hour),
			x => x.Hour, (x, v) => x.Hour = v);

	public static readonly DirectProperty<DateTimePicker, int> MinuteProperty =
		AvaloniaProperty.RegisterDirect<DateTimePicker, int>(nameof(Minute),
			x => x.Minute, (x, v) => x.Minute = v);


	private DateTime dateOnly;
	private int hour;
	private int minute;

	public DateTime DateOnly
	{
		get { return dateOnly; }
		set
		{
			SetAndRaise(DateOnlyProperty, ref dateOnly, value);
		}
	}

	public int Hour
	{
		get { return hour; }
		set
		{
			SetAndRaise(HourProperty, ref hour, value);
		}
	}
	public int Minute
	{
		get { return minute; }
		set
		{
			minute = value;
			SetAndRaise(MinuteProperty, ref minute, value);
		}
	}

	Button BtnOK;
	Button BtnCancel;
	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		BtnOK = e.NameScope.Get<Button>("BtnOK");
		BtnCancel = e.NameScope.Get<Button>("BtnCancel");

		if (BtnOK != null)
			BtnOK.Click += OnAcceptButtonClicked;
		if (BtnCancel != null)
			BtnCancel.Click += OnDismissButtonClicked;

	}

	private void OnDismissButtonClicked(object? sender, RoutedEventArgs e)
	{
		OnDismiss();
	}

	private void OnAcceptButtonClicked(object? sender, RoutedEventArgs e)
	{
		//Date = _syncDate;
		OnConfirmed();
	}

	public DateTime Value
	{
		get { return new DateTime(DateOnly.Year, DateOnly.Month, DateOnly.Day, Hour, Minute, 0); }
		set
		{
			DateOnly = value.Date;
			Hour = value.Hour;
			Minute = value.Minute;
			//OnPropertyChanged();
		}
	}
}