namespace MIP.Base.Models;

public partial class UserLastEnter
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public virtual DateTime LastEnterDate { get; set; }

	public virtual User User { get; set; } = null!;
}