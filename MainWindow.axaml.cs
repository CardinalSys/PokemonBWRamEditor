using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PkmBWRamEditor
{



	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private string processName = "";
		private CancellationTokenSource _cts;



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
			SpritesGrid.IsVisible = true;



			ram.ProcName = processName;

			ram.spritesMemoryLocationList.Clear();

			ram.ReadPos(await ram.GetBaseSpriteAddress());

			Image[] images = new Image[14];
			images[0] = Sprite1;
			images[1] = Sprite2;
			images[2] = Sprite3;
			images[3] = Sprite4;
			images[4] = Sprite5;
			images[5] = Sprite6;
			images[6] = Sprite7;
			images[7] = Sprite8;
			images[8] = Sprite9;
			images[9] = Sprite10;
			images[10] = Sprite11;
			images[11] = Sprite12;
			images[12] = Sprite13;
			images[13] = Sprite14;

			for (int i = 0; i < ram.spritesMemoryLocationList.Count; i++)
			{
				try
				{				
					var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/" + ram.spritesMemoryLocationList[i][240].ToString("X") + ".png")));
					images[i].Source = ImageToView;
				}
				catch { }

			}	
		}

		private void StartUpdateTask(Button b)
		{
			_cts = new CancellationTokenSource();
			Task.Run(() => UpdateList(_cts.Token, b), _cts.Token);
		}

		public void StopUpdateTask()
		{
			_cts?.Cancel();
		}


		public void UpdateList(CancellationToken token, Button b)
		{
			while (!token.IsCancellationRequested)
			{

				var newSpriteList = ram.CreateSpriteList(ram.AoBScanResults.Last().ToString());


				Dispatcher.UIThread.Post(() =>
				{
					ram.spritesMemoryLocationList = newSpriteList;

					UIXPos.Text = "X: " + ram.spritesMemoryLocationList[int.Parse(b.Name.ToString().Split("_")[1])][82].ToString("X");
					UIYPos.Text = "Y: " + ram.spritesMemoryLocationList[int.Parse(b.Name.ToString().Split("_")[1])][90].ToString("X");
				});


				Task.Delay(100).Wait();
			}
		}

		private void ShowSpriteInfo(object sender, RoutedEventArgs e)
		{
			StopUpdateTask();

			SpriteInfoPanel.IsVisible = true;

			Button b = (Button)e.Source;

			StartUpdateTask(b);

		}


		private void AboutClick(object sender, RoutedEventArgs e)
		{
			var psi = new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = $"/c start https://github.com/CardinalSys/PokemonBWRamEditor",
				UseShellExecute = false,
				CreateNoWindow = true
			};
			Process.Start(psi);
		}
	}

}