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

		private async void SpritesClick(object sender, RoutedEventArgs e)
		{
			MainPanel.IsVisible = false;
			SpritesPanel.IsVisible = true;

			RamAPI ram = RamAPI.GetInstance();

			ram.ProcName = processName;

			ram.spritesMemoryLocation.Clear();

			ram.ReadPos(await ram.GetBaseSpriteAddress());

			try
			{
				var ImageToView = new Bitmap("C:\\Users\\tmkuh\\source\\repos\\PkmBWRamEditor\\PkmBWRamEditor\\img\\" + ram.spritesMemoryLocation[0][240].ToString("X") + ".png");
				Sprite1.Source = ImageToView;
			} catch { }

			try
			{
				var ImageToView2 = new Bitmap("C:\\Users\\tmkuh\\source\\repos\\PkmBWRamEditor\\PkmBWRamEditor\\img\\" + ram.spritesMemoryLocation[1][240].ToString("X") + ".png");
				Sprite2.Source = ImageToView2;
			}
			catch { }

			try
			{
				var ImageToView3 = new Bitmap("C:\\Users\\tmkuh\\source\\repos\\PkmBWRamEditor\\PkmBWRamEditor\\img\\" + ram.spritesMemoryLocation[2][240].ToString("X") + ".png");
				Sprite3.Source = ImageToView3;
			}
			catch { }

			try
			{
				var ImageToView4 = new Bitmap("C:\\Users\\tmkuh\\source\\repos\\PkmBWRamEditor\\PkmBWRamEditor\\img\\" + ram.spritesMemoryLocation[3][240].ToString("X") + ".png");
				Sprite4.Source = ImageToView4;
			} catch { }


			
			
			
			

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