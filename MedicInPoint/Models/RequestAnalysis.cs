using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class RequestAnalysis
{
    public int Id { get; set; }

    public int RequestId { get; set; }

    public int AnalysisId { get; set; }

    public Analysis Analysis { get; set; } = null!;

    public Request Request { get; set; } = null!;
}
