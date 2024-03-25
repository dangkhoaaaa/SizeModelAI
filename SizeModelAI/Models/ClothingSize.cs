using System;
using System.Collections.Generic;

namespace SizeModelAI.Models;

public partial class ClothingSize
{
    public int? ClothingId { get; set; }

    public string? Size { get; set; }

    public virtual Clothing? Clothing { get; set; }
}
