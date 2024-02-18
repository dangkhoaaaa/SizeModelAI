using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;

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
	}
}
