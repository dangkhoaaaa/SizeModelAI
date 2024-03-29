using SizeModelAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SizeModelAI.Repo
{
    public class JsonClothing
    {
        
        public string IsShirt { get; set; }
        public string Type { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public string ClothingColor { get; set; }
        public string FabricMaterial { get; set; }
        public string Sleeve_length { get; set; }
        public string Collar_style { get; set; }
        public List<string> Sizes { get; set; }
    }

    public class ClothingInfoWithCount
    {
        public string Type { get; set; }
        public string Style { get; set; }
        public BitmapImage Image { get; set; }
        public int MatchCount { get; set; }
    }
}
