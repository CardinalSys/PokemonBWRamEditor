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
using System.Numerics;
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

		RamAPI ram = RamAPI.GetInstance();

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

		

		private void SpritesClick(object sender, RoutedEventArgs e)
		{

			MainPanel.IsVisible = false;
			SpritesGrid.IsVisible = true;
			SpriteInfoGrid.IsVisible = false;



			ram.ProcName = "DeSmuME_0.9.13_x64.exe";

			//Temporal
			ram.Sprites.Clear();

			ram.ReloadSpriteList();


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

			for (int i = 0; i < ram.Sprites.Count; i++)
			{
				int imgId = ram.Sprites[i].Id;
				if(imgId != 0)
				{
					try
					{
						var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/" + imgId.ToString("X") + ".png")));
						images[i].Source = ImageToView;
					}
					catch
					{
						var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/null.png")));
						images[i].Source = ImageToView;
					}
				}
				else
				{
					var ImageToView = new Bitmap(AssetLoader.Open(new Uri("avares://PkmBWRamEditor/img/null.png")));
					images[i].Source = ImageToView;
				}
				spriteNumber++;
			}	
		}

		public void SpawnCloneClick(object sender, RoutedEventArgs e)
		{
			ram.CloneSprite(index);
		}


		private void StartUpdateTask(Button b)
		{
			_cts = new CancellationTokenSource();
			Task.Run(() => UpdateInfo(_cts.Token, b), _cts.Token);
		}

		public void StopUpdateTask()
		{
			_cts?.Cancel();
		}
		public void UpdateInfo(CancellationToken token, Button b)
		{
			index = int.Parse(b.Name.ToString().Split("_")[1]);
			while (!token.IsCancellationRequested)
			{
				int XPos = (int)ram.Sprites[index].GetPos().X;
				int YPos = (int)ram.Sprites[index].GetPos().Y;

				int newAnim = (int)ram.Sprites[index].GetAnim();
				Dispatcher.UIThread.Post(() =>
				{


					UIXPos.Text = "X: " + XPos;
					UIYPos.Text = "Y: " + YPos;
					SpriteId.Text = "Sprite ID: " + ram.Sprites[index].Id;
					AnimationId.Text = "Animation: " + newAnim;


					if(int.TryParse(XNewPos.Text, out XPos))
						ram.Sprites[index].SetXPos(XPos);

					if (int.TryParse(YNewPos.Text, out YPos))
						ram.Sprites[index].SetYPos(YPos);

					if (int.TryParse(NewAnimationId.Text, out newAnim))
						ram.Sprites[index].SetAnim(newAnim);

					NewAnimationId.Text = "";

				});

				Task.Delay(100).Wait();
			}
		}


		private void ShowSpriteInfo(object sender, RoutedEventArgs e)
		{
			StopUpdateTask();

			YNewPos.Text = null;
			XNewPos.Text = null;

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