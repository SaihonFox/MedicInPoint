namespace MedicInPoint.Models;

public partial class UserData
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime LastEnter { get; set; }

    public virtual User User { get; set; } = null!;
}
