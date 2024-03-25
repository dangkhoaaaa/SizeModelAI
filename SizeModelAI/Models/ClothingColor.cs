using System;
using System.Collections.Generic;

namespace SizeModelAI.Models;

public partial class ClothingColor
{
    public int? ClothingId { get; set; }

    public string? Color { get; set; }

    public virtual Clothing? Clothing { get; set; }
}
