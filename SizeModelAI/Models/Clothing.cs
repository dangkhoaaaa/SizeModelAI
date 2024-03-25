using System;
using System.Collections.Generic;

namespace SizeModelAI.Models;

public partial class Clothing
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? Style { get; set; }

    public string? Fit { get; set; }

    public string? FabricMaterial { get; set; }

    public string? Image { get; set; }
}
