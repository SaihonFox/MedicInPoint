using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;

using Lucdem.Avalonia.SourceGenerators.Attributes;

namespace Medic.Theme.Controls;

[PseudoClasses(":contentless")]
public partial class Separator : ContentControl
{
	[AvaStyledProperty]
	private Orientation _orientation = Orientation.Horizontal;
}