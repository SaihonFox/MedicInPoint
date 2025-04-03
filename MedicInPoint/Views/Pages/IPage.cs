using Avalonia.Controls;

namespace MedicInPoint.Views.Pages;

public interface IPage<TWindowOwner> where TWindowOwner : WindowBase
{
	public TWindowOwner TOwner { get; }
}