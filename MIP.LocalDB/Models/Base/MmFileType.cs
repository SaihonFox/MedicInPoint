namespace MIP.Base.Models;

public partial class MmFileType
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public virtual ICollection<MmFile> MmFiles { get; set; } = [];
}