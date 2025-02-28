using Avalonia.Controls;
using Avalonia.Media;

using Lucdem.Avalonia.SourceGenerators.Attributes;

namespace Medic.Theme.Controls.Custom;

public partial class MenuItem : Button
{
	[AvaStyledProperty]
	private IImage? imageSource = null;

	[AvaStyledProperty]
	private string? imageUri = null;

	[AvaStyledProperty]
	private string text = "";
}