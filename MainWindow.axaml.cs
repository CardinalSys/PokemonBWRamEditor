using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PkmBWRamEditor
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private string processName = "";



		private async void OpenClick(object sender, RoutedEventArgs e)
		{

			var topLevel = TopLevel.GetTopLevel(this);


			var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				Title = "Select emulator .exe",
				AllowMultiple = false
			});

			if (files.Count >= 1)
			{
				processName = files[0].Name;
			}
		}

		RamAPI ram = RamAPI.GetInstance();

		private async void SpritesClick(object sender, RoutedEventArgs e)
		{
			MainPanel.IsVisible = false;
			SpritesPanel.IsVisible = true;



			ram.ProcName = processName;

			ram.spritesMemoryLocation.Clear();

			ram.ReadPos(await ram.GetBaseSpriteAddress());

			Image[] images = new Image[5];
			images[0] = Sprite1;
			images[1] = Sprite2;
			images[2] = Sprite3;
			images[3] = Sprite4;
			images[4] = Sprite5;

			for (int i = 0; i < ram.spritesMemoryLocation.Count; i++)
			{
				try
				{
					var ImageToView = new Bitmap("C:\\Users\\tmkuh\\source\\repos\\PkmBWRamEditor\\PkmBWRamEditor\\img\\" + ram.spritesMemoryLocation[i][240].ToString("X") + ".png");
					images[i].Source = ImageToView;
				}
				catch { }

			}	
		}

		private void ShowSpriteInfo(object sender, RoutedEventArgs e)
		{
			SpriteInfoPanel.IsVisible = true;

			Button b = (Button)e.Source;

			UIXPos.Text = "X: " + ram.spritesMemoryLocation[int.Parse(b.Name.ToString().Split("_")[1])][82].ToString("X");
			UIYPos.Text = "Y: " + ram.spritesMemoryLocation[int.Parse(b.Name.ToString().Split("_")[1])][90].ToString("X");
		}


		private void AboutClick(object sender, RoutedEventArgs e)
		{
			var psi = new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = $"/c start https://github.com/CardinalSys",
				UseShellExecute = false,
				CreateNoWindow = true
			};
			Process.Start(psi);
		}
	}
}