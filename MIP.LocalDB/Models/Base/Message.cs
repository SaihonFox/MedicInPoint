namespace MIP.Base.Models;

public partial class Message
{
	public int Id { get; set; }

	public int PatientId { get; set; }

	public virtual ICollection<MessagesMessage> MessagesMessages { get; set; } = [];

	public virtual Patient Patient { get; set; } = null!;
}