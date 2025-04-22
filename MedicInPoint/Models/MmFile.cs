using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class MmFile
{
    public int Id { get; set; }

    public int MessagesMessageId { get; set; }

    public byte[]? File { get; set; }

    public int MmFileTypeId { get; set; }

    public string FileName { get; set; } = null!;

    public string? FileExtension { get; set; }

    public MessagesMessage MessagesMessage { get; set; } = null!;

    public MmFileType MmFileType { get; set; } = null!;
}
