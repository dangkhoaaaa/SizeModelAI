using Newtonsoft.Json;
using SizeModelAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SizeModelAI
{
    /// <summary>
    /// Interaction logic for PreClothing.xaml
    /// </summary>
    public partial class PreClothing : Window
    {
        private Clothing clothing;

        public PreClothing(string json)
        {
            InitializeComponent();

            // Loại bỏ dấu ngoặc đơn từ chuỗi JSON
            json = json.Replace("{", "").Replace("}", "");

            // Tách các cặp key-value thành mảng
            string[] pairs = json.Split('|');

            // Tạo một đối tượng JsonClothing và gán giá trị cho các thuộc tính
            JsonClothing clothing = new JsonClothing();
            foreach (var pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                string key = keyValue[0];
                string value = keyValue[1];

                switch (key)
                {
                    case "Type":
                        clothing.Type = value;
                        break;
                    case "Style":
                        clothing.Style = value;
                        break;
                    case "Fit":
                        clothing.Fit = value;
                        break;
                    case "ClothingColor":
                        clothing.ClothingColor = new List<string> { value };
                        break;
                    case "FabricMaterial":
                        clothing.FabricMaterial = value;
                        break;
                    case "Sizes":
                        clothing.Sizes = new List<string> { value };
                        break;
                }
            }

            // In ra thông tin của đối tượng JsonClothing
            Console.WriteLine($"Type: {clothing.Type}");
            Console.WriteLine($"Style: {clothing.Style}");
            Console.WriteLine($"Fit: {clothing.Fit}");
            Console.WriteLine($"ClothingColor: {string.Join(", ", clothing.ClothingColor)}");
            Console.WriteLine($"FabricMaterial: {clothing.FabricMaterial}");
            Console.WriteLine($"Sizes: {string.Join(", ", clothing.Sizes)}");
            ShowClothingInfo();
        }

        // Phương thức để hiển thị thông tin của quần áo trên giao diện
        private void ShowClothingInfo()
        {
           
        }
    }
}
