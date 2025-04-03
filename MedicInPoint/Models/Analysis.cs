using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class Analysis
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Preparation { get; set; }

    public string ResultsAfter { get; set; } = null!;

    public string Biomaterial { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<AdBlock> AdBlocks { get; set; } = new List<AdBlock>();

    public virtual ICollection<AnalysisCategoriesList> AnalysisCategoriesLists { get; set; } = new List<AnalysisCategoriesList>();

    public virtual ICollection<PatientAnalysisCartItem> PatientAnalysisCartItems { get; set; } = new List<PatientAnalysisCartItem>();
}
