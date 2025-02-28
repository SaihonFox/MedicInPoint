namespace MedicInPoint.Models;

public partial class Patient
{
	public string FullName => $"{Surname} {Name}{(Patronym != null ? " " + Patronym : "")}";
}