using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SkiaSharp;
using System.Threading;
using Memory;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace PkmBWRamEditor
{
	public class Multiplayer
	{
		private RamAPI ram;
		private static Multiplayer instance;
		private bool isSpawned = false;
		private int index;
		public static Multiplayer GetInstance()
		{
			if (instance == null)
				instance = new Multiplayer();

			return instance;
		}

		public Multiplayer()
		{
			ram = RamAPI.GetInstance();
		}

		public void SpawnSecondPlayer()
		{
			for (int i = 0; i < ram.Sprites.Count; i++)
			{
				if (ram.Sprites[i].Id == 1)
				{
					index = ram.CloneSprite(i);
					break;
				}
					
			}
			if(index > 0)
				isSpawned = true;

			Task.Run(() => Controller());
		}

		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int key);

		public void Controller()
		{
			Console.WriteLine("Presiona 'ESC' para salir...");

			while (isSpawned)
			{
				

				bool isUpPressed = (GetAsyncKeyState(0x62) & 0x8000) != 0;
				bool isDownPressed = (GetAsyncKeyState(0x68) & 0x8000) != 0;
				bool isRightPressed = (GetAsyncKeyState(0x66) & 0x8000) != 0;
				bool isLeftPressed = (GetAsyncKeyState(0x64) & 0x8000) != 0;

				if (isUpPressed)
				{
					ram.Sprites[index].SetYPos((byte)ram.Sprites[index].GetPos().Y - 1);
					Thread.Sleep(100);
				}

				if (isDownPressed)
				{
					ram.Sprites[index].SetYPos((byte)ram.Sprites[index].GetPos().Y + 1);
					Thread.Sleep(100);
				}

				if (isRightPressed)
				{
					ram.Sprites[index].SetXPos((byte)ram.Sprites[index].GetPos().X + 1);
					Thread.Sleep(100);
				}

				if (isLeftPressed)
				{
					ram.Sprites[index].SetXPos((byte)ram.Sprites[index].GetPos().X - 1);
					Thread.Sleep(100);
				}

				if ((GetAsyncKeyState(0x1B) & 0x8000) != 0) 
				{
					break;
				}
			}
		}
	}
}


