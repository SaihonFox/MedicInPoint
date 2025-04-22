using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class PatientAnalysisCartItem
{
    public int Id { get; set; }

    public int AnalysisId { get; set; }

    public int PatientAnalysisCartId { get; set; }

    public Analysis Analysis { get; set; } = null!;

    public PatientAnalysisCart PatientAnalysisCart { get; set; } = null!;
}
