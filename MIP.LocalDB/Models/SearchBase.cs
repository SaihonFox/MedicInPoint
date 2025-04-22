using System.ComponentModel.DataAnnotations;

namespace MIP.LocalDB.Models;

public abstract class SearchBase
{
	[Key]
	public int id { get; set; }

	public string name { get; set; } = string.Empty;
}
