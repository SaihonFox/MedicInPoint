namespace MIP.Base.Models;

public partial class RequestState
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public virtual ICollection<Request> Requests { get; set; } = [];
}