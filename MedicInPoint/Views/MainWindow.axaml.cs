using Avalonia.Controls;
using Avalonia.Input;

using MedicInPoint.ViewModels;

namespace MedicInPoint.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		AddHandler(DragDrop.DropEvent, Drop);
	}

	void Drop(object? sender, DragEventArgs e)
	{
		/*
		var control = sender as UserControl;
		var sb = new StringBuilder($"TypeCode: {e.DragEffects.GetTypeCode()}\n");
		sb.AppendLine($"Pos: {e.GetPosition(control)}");
		sb.AppendLine($"Data formats: \n\t- {string.Join(",\n\t- ", e.Data.GetDataFormats().Select(df => {
			string value = "unknown or not implemented";
			if (e.Data.Get(df)?.GetType() == typeof(byte[]))
				value = Encoding.Unicode.GetString(e.Data.Get(df) as byte[]);
			if (e.Data.Get(df)?.GetType().IsArray == true)
				value = string.Join(";", e.Data.Get(df) as dynamic);
			return $"{df}: type={e.Data.Get(df)}: value={value}";
		}))}");
		sb.AppendLine($"Files: {string.Join(" | ", e.Data.GetFiles()?.Select(f => $"Name: {f.Name}, Path: {f.Path}") ?? [])}");
		control.Content = new TextBlock { Text = sb.ToString(), TextWrapping = Avalonia.Media.TextWrapping.WrapWithOverflow }; */
	}
}