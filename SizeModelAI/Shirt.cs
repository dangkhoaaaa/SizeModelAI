using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeModelAI
{
    public class Shirt
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Style { get; set; }
        public string Fit { get; set; }
        public List<string> ClothingColors { get; set; } // Mảng màu sắc của quần áo
        public string FabricMaterial { get; set; }
        public List<string> Sizes { get; set; } // Mảng các kích cỡ
        public string Image { get; set; } // Trường ảnh
    }
}
