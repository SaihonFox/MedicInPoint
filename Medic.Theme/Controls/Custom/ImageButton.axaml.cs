using Avalonia.Controls;
using Avalonia.Media;

using Lucdem.Avalonia.SourceGenerators.Attributes;

namespace Medic.Theme.Controls.Custom;

public partial class ImageButton : Button
{
	[AvaStyledProperty]
	private BoxShadow? _bgShadow;

	protected override Type StyleKeyOverride => typeof(ImageButton);
}