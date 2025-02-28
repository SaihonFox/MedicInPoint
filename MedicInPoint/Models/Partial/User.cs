namespace MedicInPoint.Models;

public partial class User
{
	public string FullName => $"{Surname} {Name}{(Patronym != null ? " " + Patronym : "")}";
}