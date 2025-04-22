namespace MIP.Base.Models;

public partial class MessagesMessage
{
	public int Id { get; set; }

	public int? MessagesId { get; set; }

	public string? Message { get; set; }

	public DateTime Time { get; set; }

	public int? UserId { get; set; }

	public int? PatientId { get; set; }

	public virtual Message? Messages { get; set; }

	public virtual ICollection<MmFile> MmFiles { get; set; } = [];

	public virtual Patient? Patient { get; set; }

	public virtual User? User { get; set; }
}