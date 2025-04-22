using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class PatientDatum
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public double? Weight { get; set; }

    public double? Height { get; set; }

    public Patient Patient { get; set; } = null!;
}
