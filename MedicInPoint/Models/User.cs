using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class User
{
	public int Id { get; set; }

	public string Surname { get; set; } = null!;

	public string Name { get; set; } = null!;

	public string? Patronym { get; set; }

	public DateOnly Birthday { get; set; }

	public string? Passport { get; set; }

	public string? Phone { get; set; }

	public bool? IsBlocked { get; set; }

	public sbyte? Post { get; set; }

	public string Login { get; set; } = null!;

	public string Password { get; set; } = null!;

	public byte[]? Image { get; set; }

	public ICollection<AnalysisOrder> AnalysisOrders { get; set; } = [];

	public ICollection<MessagesMessage> MessagesMessages { get; set; } = [];

	public ICollection<UserLastEnter> UserLastEnters { get; set; } = [];

	public ICollection<UserStatus> UserStatuses { get; set; } = [];
}