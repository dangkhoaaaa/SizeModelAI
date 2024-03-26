using Newtonsoft.Json;
using SizeModelAI;
using SizeModelAI.Repo;
using System;
using System.Collections.Generic;
using System.IO;
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

        UnitOfWork unitOfWork = new UnitOfWork();
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
                string key = keyValue[0].Trim();
                string value = keyValue[1];

                switch (key)
                {
                    case "isShirt":
                        clothing.IsShirt = value;
                        break;
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
                        clothing.ClothingColor = value;
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
            //Console.WriteLine($"Type: {clothing.Type}");
            //Console.WriteLine($"Style: {clothing.Style}");
            //Console.WriteLine($"Fit: {clothing.Fit}");
            //Console.WriteLine($"ClothingColor: {string.Join(", ", clothing.ClothingColor)}");
            //Console.WriteLine($"FabricMaterial: {clothing.FabricMaterial}");
            //Console.WriteLine($"Sizes: {string.Join(", ", clothing.Sizes)}");
            if (clothing.IsShirt.Equals("Yes"))
            {
                ShowClothingInfo(clothing);
            }
            else
            {
                MessageBox.Show("This is not Shirt");
            }
        }


        private void ShowClothingInfo(JsonClothing obj)
        {
            //var list = unitOfWork.ClothingRepository.Get(filter: c => c.Type == obj.Type || c.Style == obj.Style || c.Fit == obj.Fit || c.Color == obj.ClothingColor || c.FabricMaterial == obj.FabricMaterial);

            //List<ClothingInfo> clothingInfos = new List<ClothingInfo>();

            //foreach (var item in list)
            //{
            //    if (!string.IsNullOrEmpty(item.Image))
            //    {
            //        byte[] imageBytes = Convert.FromBase64String(item.Image);
            //        BitmapImage bitmap = new BitmapImage();
            //        bitmap.BeginInit();
            //        bitmap.StreamSource = new MemoryStream(imageBytes);
            //        bitmap.EndInit();

            //        clothingInfos.Add(new ClothingInfo
            //        {
            //            Type = item.Type,
            //            Style = item.Style,
            //            Image = bitmap
            //        });
            //    }
            //}

            //listdata.ItemsSource = clothingInfos;
            var list = unitOfWork.ClothingRepository.Get(filter: c => c.Type == obj.Type || c.Style == obj.Style || c.Fit == obj.Fit || c.Color == obj.ClothingColor || c.FabricMaterial == obj.FabricMaterial);

            List<ClothingInfoWithCount> clothingInfos = new List<ClothingInfoWithCount>();

            foreach (var item in list)
            {
                int matchCount = 0;

                if (!string.IsNullOrEmpty(item.Type) && item.Type == obj.Type)
                    matchCount++;
                if (!string.IsNullOrEmpty(item.Style) && item.Style == obj.Style)
                    matchCount++;
                if (!string.IsNullOrEmpty(item.Fit) && item.Fit == obj.Fit)
                    matchCount++;
                if (!string.IsNullOrEmpty(item.Color) && item.Color == obj.ClothingColor)
                    matchCount++;
                if (!string.IsNullOrEmpty(item.FabricMaterial) && item.FabricMaterial == obj.FabricMaterial)
                    matchCount++;

                if (matchCount > 0)
                {
                    byte[] imageBytes = Convert.FromBase64String(item.Image);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(imageBytes);
                    bitmap.EndInit();

                    clothingInfos.Add(new ClothingInfoWithCount
                    {
                       Type = item.Type,
                       Style = item.Style,
                       Image = bitmap,
                       MatchCount = matchCount
                    });
                }
            }

            // Order by MatchCount in descending order
            clothingInfos = clothingInfos.OrderByDescending(c => c.MatchCount).ToList();

            listdata.ItemsSource = clothingInfos;
        }
       


    }
}
