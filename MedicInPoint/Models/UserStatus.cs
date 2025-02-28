namespace MedicInPoint.Models;

public partial class UserStatus
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public bool IsFired { get; set; }

    public User User { get; set; } = null!;
}