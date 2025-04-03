using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class MessagesMessage
{
    public int Id { get; set; }

    public int? MessagesId { get; set; }

    public string? Message { get; set; }

    public DateTime Time { get; set; }

    public int? UserId { get; set; }

    public int? PatientId { get; set; }

    public Message? Messages { get; set; }

    public ICollection<MmFile> MmFiles { get; set; } = [];

    public Patient? Patient { get; set; }

    public User? User { get; set; }
}
