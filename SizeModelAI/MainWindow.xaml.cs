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
using AForge.Video.DirectShow;
using AForge.Video;

namespace SizeModelAI
{
	public partial class MainWindow : System.Windows.Window
	{
        private VideoCaptureDevice videoCaptureDevice;
        private string saveFolderPath;
        private System.Drawing.Bitmap currentFrame;
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
            InitializeWebcam();
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

                CallAPIProcessAsync(base64Image, filePath);
            }
}
        private async Task CallAPIProcessAsync(string base64Image,string filePath)
        { // Tạo JSON request
            string questionText = "what is picture,please return this JSON: {   type: Shirt,   " +
                " style: Casual,    fit: Regular fit,    clothing_color:" +
                " example[White, Blue, Black, ....],abric_material: Cotton,    sizes: [S, M, L,XL]  }";

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

            string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent?key=AIzaSyArVfvy9rHMaUh7_nOwkruwRTGh8abbQJY";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.PostAsync(apiUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(responseBody);
                    JArray candidates = (JArray)jsonResponse["candidates"];
                    string result = "";

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
                                        result = text;
                                    }
                                }
                            }
                        }
                    }

                    // Hiển thị kết quả trả về từ API
                    MessageBox.Show("Answer " + result);

                    // Hiển thị ảnh đã chụp lên giao diện người dùng
                    Mat image = Cv2.ImRead(filePath);
                    BitmapSource bitmapSource = BitmapSourceConverter.ToBitmapSource(image);
                    imageView.Source = bitmapSource;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
        private void InitializeWebcam()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoCaptureDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;

                videoCaptureDevice.Start();
            }
            else
            {
                MessageBox.Show("Không tìm thấy webcam.");
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (currentFrame != null)
            {
                currentFrame.Dispose();
            }

            currentFrame = (System.Drawing.Bitmap)eventArgs.Frame.Clone();

            Dispatcher.Invoke(() =>
            {
                BitmapImage bitmapImage = ConvertBitmapToBitmapImage(currentFrame);
                imageControl.Source = bitmapImage;
            });
        }

        private BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            CaptureImage();
        }
        private string saveFolderPath1 = @"D:\";
        private async void CaptureImage()
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                if (currentFrame != null)
                {
                    string fileName = $"image_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    string filePath = Path.Combine(saveFolderPath1, fileName);

                    currentFrame.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    MessageBox.Show($"Ảnh đã được chụp và lưu tại {filePath}");

                    // Đọc dữ liệu ảnh đã chụp
                    byte[] imageData = File.ReadAllBytes(filePath);
                    string base64Image = Convert.ToBase64String(imageData);
                    CallAPIProcessAsync(base64Image,filePath);
                   
                    
                }
            }
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {



        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.SignalToStop();
                videoCaptureDevice.WaitForStop();
                videoCaptureDevice = null;
            }

            if (currentFrame != null)
            {
                currentFrame.Dispose();
            }

            base.OnClosing(e);
        }
    }
}
