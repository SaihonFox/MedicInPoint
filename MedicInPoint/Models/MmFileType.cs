using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class MmFileType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<MmFile> MmFiles { get; set; } = [];
}
