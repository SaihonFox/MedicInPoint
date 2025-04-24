using Newtonsoft.Json;

namespace MedicInPoint.Models;

public partial class User
{
	[JsonIgnore]
	public string FullName => $"{Surname} {Name}{(Patronym != null ? " " + Patronym : "")}";
}