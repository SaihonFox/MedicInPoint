using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class AnalysisCategoriesList
{
    public int Id { get; set; }

    public int AnalysisId { get; set; }

    public int AnalysisCategoryId { get; set; }

    public Analysis Analysis { get; set; } = null!;

    public AnalysisCategory AnalysisCategory { get; set; } = null!;
}
