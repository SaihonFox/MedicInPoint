using System;

namespace MedicInPoint.Models;

public partial class UserLastEnter
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public DateOnly LastEnterDate { get; set; }

	public virtual User User { get; set; } = null!;
}