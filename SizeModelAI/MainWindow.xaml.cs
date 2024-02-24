using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SizeModelAI
{
	public partial class MainWindow : System.Windows.Window
	{
		public MainWindow()
		{
			InitializeComponent();
			// Check the existence of the haarcascade_fullbody.xml file
			string cascadeFileName = "haarcascade_fullbody.xml";
			string cascadeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cascadeFileName);

			if (!File.Exists(cascadeFilePath))
			{
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				string parentDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.FullName;
				// Copy the file from the application's root directory
				string sourceFilePath = Path.Combine(parentDirectory, "haarcascade_fullbody.xml"); // Path to the source file
				File.Copy(sourceFilePath, cascadeFilePath);
			}
		}

		private void LoadImage_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

			if (openFileDialog.ShowDialog() == true)
			{
				string filePath = openFileDialog.FileName;

				// Read the image and convert it to an OpenCV Mat object
				Mat image = Cv2.ImRead(filePath);
				// Check the existence of the haarcascade_fullbody.xml file
				string cascadeFileName = "haarcascade_fullbody.xml";
				string cascadeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cascadeFileName);

				if (!File.Exists(cascadeFilePath))
				{
					MessageBox.Show("haarcascade_fullbody.xml not found. Please make sure the file exists.");
					return;
				}
				// Create a body detector
				using (var bodyCascade = new CascadeClassifier("haarcascade_fullbody.xml"))
				{

					// Detect people in the image
					OpenCvSharp.Rect[] bodies = bodyCascade.DetectMultiScale(image, 1.1, 3, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));

					if (bodies.Length > 0)
					{


						// Measure the width and height of the first person in the image
						OpenCvSharp.Rect firstBody = bodies[0];
						double personWidth = firstBody.Width;
						double personHeight = firstBody.Height;

						//MessageBox.Show($"Width of person: {personWidth}, Height of person: {personHeight}");
						string size;
						if (personWidth < 50 && personHeight < 150)
						{
							size = "S"; // Small size clothes
						}
						else if (personWidth < 70 && personHeight < 170)
						{
							size = "M"; // Medium size clothes
						}
						else if (personWidth < 90 && personHeight < 190)
						{
							size = "L"; // Large size clothes
						}
						else
						{
							size = "XL"; // Extra large size clothes
						}
						// Display the image in the Image control
						// Display the body of the person in the image
						foreach (var body in bodies)
						{
							Cv2.Rectangle(image, body, new Scalar(0, 255, 0), 2);
						}

						// Display the image in the Image control
						BitmapSource bitmapSource = BitmapSourceConverter.ToBitmapSource(image);
						imageView.Source = bitmapSource;
						MessageBox.Show($"Predicted clothing size: {size}");
					}
					else
					{
						// If no person is in the image
						MessageBox.Show("Need Image full Body");
					}
				}

				// Free memory after use
				image.Dispose();
			}
		}

		private async void LoadImageAI_Click(object sender, RoutedEventArgs e)
{
    var openFileDialog = new OpenFileDialog();
    openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

    if (openFileDialog.ShowDialog() == true)
    {
        string filePath = openFileDialog.FileName;
        byte[] imageData = File.ReadAllBytes(filePath);

        // Convert image data to base64
        string base64Image = Convert.ToBase64String(imageData);

        // Determine the selected question from the ComboBox
        string selectedQuestion = questionComboBox.SelectedItem.ToString();
        string questionText = selectedQuestion;

				
				// Create JSON request
				string jsonRequest = @"{
            ""contents"":[
                {
                    ""parts"":[
                        {""text"": """ + questionText + @"""},
                        {
                            ""inline_data"": {
                                ""mime_type"":""image/jpeg"",
                                ""data"": """ + base64Image + @"""
                            }
                        }
                    ]
                }
            ]
        }";

        // Send request to API
        string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent?key=AIzaSyArVfvy9rHMaUh7_nOwkruwRTGh8abbQJY";
        using (var httpClient = new HttpClient())
        {
            try
            {
						string result= "";
                var response = await httpClient.PostAsync(apiUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
					
						//MessageBox.Show("Result: " + responseBody);

						// Phân tích JSON
						JObject jsonResponse = JObject.Parse(responseBody);

						// Lấy danh sách các ứng viên từ JSON
						JArray candidates = (JArray)jsonResponse["candidates"];

						// Lặp qua từng ứng viên và hiển thị nội dung text
						foreach (JToken candidate in candidates)
						{
							JToken content = candidate["content"];
							if (content != null)
							{
								JArray parts = (JArray)content["parts"];
								if (parts != null && parts.Count > 0)
								{
									foreach (JToken part in parts)
									{
										string text = (string)part["text"];
										if (text != null)
										{
											//MessageBox.Show("Descripton about picture: " + text);
											result = text;
										}
									}
								}
							}
						}
						Mat image = Cv2.ImRead(filePath);
						BitmapSource bitmapSource = BitmapSourceConverter.ToBitmapSource(image);
						imageView.Source = bitmapSource;
						MessageBox.Show("Answer " + result);

					}
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
	}
}
