using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
			StopUpdateTask();

			MainPanel.IsVisible = false;
			SpritesGrid.IsVisible = true;
			SpriteInfoGrid.IsVisible = false;



			ram.ProcName = processName;

			ram.spritesMemoryLocationList.Clear();

			ram.ReadPos(await ram.GetBaseSpriteAddress());

			Image[] images = new Image[14];
			images[0] = Sprite0;
			images[1] = Sprite1;
			images[2] = Sprite2;
			images[3] = Sprite3;
			images[4] = Sprite4;
			images[5] = Sprite5;
			images[6] = Sprite6;
			images[7] = Sprite7;
			images[8] = Sprite8;
			images[9] = Sprite9;
			images[10] = Sprite10;
			images[11] = Sprite11;
			images[12] = Sprite12;
			images[13] = Sprite13;

			for (int i = 0; i < images.Length; i++)
			{
				try
				{
					string imgId = ram.spritesMemoryLocationList[i][240].ToString("X");
					var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/" + imgId + ".png")));
					images[i].Source = ImageToView;
				}
				catch {
					var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/null.png")));
					images[i].Source = ImageToView;
				}

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


		private void UpdatePos(string text, int index, int pos, int col)
		{
			byte value;

			if (text != null)
			{
				if (byte.TryParse(text, out value))
				{
					ram.WriteRam(pos + (256 * index), value);
					ram.WriteRam(col + (256 * index), value);
				}
			}
		}

		private void UpdateSpriteId(string text, int index, int offset)
		{
			byte value;

			if (text != ram.spritesMemoryLocationList[index][offset].ToString("X"))
			{

				if (byte.TryParse(text, out value))
				{
					ram.WriteRam(offset + (256 * index), value);
				}
			}
		}


		public void UpdateList(CancellationToken token, Button b)
		{
			while (!token.IsCancellationRequested)
			{

				var newSpriteList = ram.CreateSpriteList(ram.AoBScanResults.Last().ToString());


				Dispatcher.UIThread.Post(() =>
				{
					
					int index = int.Parse(b.Name.ToString().Split("_")[1]) + 1;


					UpdatePos(XNewPos.Text, index, 26, 16);

					UpdatePos(YNewPos.Text, index, 34, 20);


					UpdateSpriteId(NewSpriteID.Text, index, 184);

					
					if (FreezeXPos.IsChecked == true)
						ram.FreezeXPos();
					else
						ram.UnFreezeXPos();

					if (FreezeYPos.IsChecked == true)
						ram.FreezeXPos();
					else
						ram.UnFreezeXPos();

					
					ram.spritesMemoryLocationList = newSpriteList;

					UIXPos.Text = "X: " + ram.spritesMemoryLocationList[index][26].ToString("X");			
					UIYPos.Text = "Y: " + ram.spritesMemoryLocationList[index][34].ToString("X");
					UIZPos.Text = "Z: " + ram.spritesMemoryLocationList[index][30].ToString("X");
					SpriteId.Text = "Sprite ID: " + ram.spritesMemoryLocationList[index][184].ToString("X");

				});

				Task.Delay(100).Wait();
			}
		}



		private void ShowSpriteInfo(object sender, RoutedEventArgs e)
		{
			StopUpdateTask();

			NewSpriteID.Text = null;
			YNewPos.Text = null;
			XNewPos.Text = null;
			zNewPos.Text = null;

			SpriteInfoGrid.IsVisible = true;

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