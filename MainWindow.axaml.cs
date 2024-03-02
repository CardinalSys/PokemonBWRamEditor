using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Microsoft.CodeAnalysis;
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

		int index;
		int spriteNumber = 0;

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

			spriteNumber = 0;

			for (int i = 0; i < images.Length; i++)
			{
				try
				{
					string imgId = ram.spritesMemoryLocationList[i][239].ToString("X");
					var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/" + imgId + ".png")));
					images[i].Source = ImageToView;
					spriteNumber++;
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

		public void SpawnCloneBtn(object sender, RoutedEventArgs e)
		{
			ram.WriteBytes(256 * spriteNumber, ram.spritesMemoryLocationList[index]);
		}


		public void UpdateList(CancellationToken token, Button b)
		{
			index = int.Parse(b.Name.ToString().Split("_")[1]);
			byte xFreezeValue = 0;
			byte yFreezeValue = 0;
			byte AnimationFreezeValue = 0;
			bool isXFrozen = false;
			bool isYFrozen = false;
			bool isAnimationFrozen = false;
			string newXValueString = null;
			string newYValueString = null;
			string newAnimation = null;

			while (!token.IsCancellationRequested)
			{			
				var newSpriteList = ram.CreateSpriteList((ram.AoBScanResults.Last() + 57).ToString());

				


				Dispatcher.UIThread.Post(() =>
				{
					UIXPos.Text = "X: " + newSpriteList[index][81].ToString("X");
					UIYPos.Text = "Y: " + newSpriteList[index][89].ToString("X");
					AnimationId.Text = "Animation: " + newSpriteList[index][35].ToString("X");

					isXFrozen = FreezeXPos.IsChecked == true;
					isYFrozen = FreezeYPos.IsChecked == true;
					isAnimationFrozen = FreezeAnimationId.IsChecked == true;

					newXValueString = XNewPos.Text;
					newYValueString = YNewPos.Text;
					newAnimation = NewAnimationId.Text;

				});

				//Change x pos
				if(newXValueString != null)
				{
					byte newXValue;
					if (byte.TryParse(newXValueString, out newXValue))
					{
						ram.WriteRam(81 + (256 * index), newXValue);
						ram.WriteRam(71 + (256 * index), newXValue);
					}
				}

				//Change Y pos
				if (newYValueString != null)
				{
					byte newYValue;
					if (byte.TryParse(newYValueString, out newYValue))
					{
						ram.WriteRam(89 + (256 * index), newYValue);
						ram.WriteRam(75 + (256 * index), newYValue);
					}
				}

				//Change Animation
				if (newAnimation != null)
				{
					byte newValue;
					if (byte.TryParse(newAnimation, out newValue))
					{
						ram.WriteRam(35 + (256 * index), newValue);
					}
				}

				//Freeze x pos
				if (isXFrozen)
					ram.WriteRam(81 + (256 * index), xFreezeValue);
				else
				{
					xFreezeValue = newSpriteList[index][81];
					if(newSpriteList[index][71] != newSpriteList[index][81])
						ram.WriteRam(71 + (256 * index), newSpriteList[index][81]);
				}

				//Freeze y pos
				if (isYFrozen)
					ram.WriteRam(89 + (256 * index), yFreezeValue);
				else
				{
					yFreezeValue = newSpriteList[index][89];
					if (newSpriteList[index][75] != newSpriteList[index][89])
						ram.WriteRam(75 + (256 * index), newSpriteList[index][89]);
				}

				//Freeze Animation
				if (isAnimationFrozen)
					ram.WriteRam(35 + (256 * index), AnimationFreezeValue);
				else
				{
					AnimationFreezeValue = newSpriteList[index][35];
				}

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