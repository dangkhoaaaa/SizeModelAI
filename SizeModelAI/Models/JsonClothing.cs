using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeModelAI.Models
{
    public class JsonClothing
    {
        public string Type { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public List<string> ClothingColor { get; set; }
        public string FabricMaterial { get; set; }
        public List<string> Sizes { get; set; }
    }
}
