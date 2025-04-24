namespace MedicInPoint.Models;

public partial class UserLastEnter
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime LastEnterDate { get; set; }

    public User User { get; set; } = null!;
}