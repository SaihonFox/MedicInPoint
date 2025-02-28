using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class Message
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public ICollection<MessagesMessage> MessagesMessages { get; set; } = [];

    public Patient Patient { get; set; } = null!;
}