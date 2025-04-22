using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class AnalysisCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<AnalysisCategoriesList> AnalysisCategoriesLists { get; set; } = [];
}
