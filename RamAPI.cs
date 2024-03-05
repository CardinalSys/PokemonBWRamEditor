using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Memory;
using PkmBWRamEditor;

namespace PkmBWRamEditor
{
	internal class RamAPI
	{

		private static RamAPI instance;

        public string ProcName { get; set; }

        private long baseSpritesAddress = 0x14AB675F9;
		private Mem procMem = new Mem();

		public List<Sprite> Sprites = new List<Sprite>();

		public static RamAPI GetInstance()
		{
			if (instance == null)
				instance = new RamAPI();

			return instance;
		}

		public byte[] ReadBytes(long address)
		{
			byte[] bytes = new byte[256];
			if (IsProcessOpen())
				bytes = procMem.ReadBytes(address.ToString("X"), 256);
			return bytes;
		}

		public int ReadByte(long address)
		{
			int result = -1;
			if (IsProcessOpen())
				result = procMem.ReadByte(address.ToString("X"));
			return result;
		}

		public void WriteBytes(long address, byte[] bytes)
		{
			if (IsProcessOpen())
				procMem.WriteBytes(address.ToString("X"), bytes);
		}

		public void WriteByte(long address, int value)
		{
			if (IsProcessOpen())
				procMem.WriteMemory(address.ToString("X"), "byte", value.ToString());
		}

		public bool IsProcessOpen()
		{
			return procMem.OpenProcess(ProcName);
		}

		public void ReloadSpriteList()
		{
			long currentSpriteAddrees = baseSpritesAddress;
			for (int i = 0; i < 14; i++)
			{
				Sprite sprite = new Sprite(currentSpriteAddrees);

				Sprites.Add(sprite);

				currentSpriteAddrees += 256;

			}

		}

		public void CloneSprite(int index)
		{
			Sprite sprite = Sprites[index];

			for(int i = 0; i < Sprites.Count; i++)
			{
				if (Sprites[i].Id == 0)
				{
					Sprites[i] = sprite;
					WriteBytes(baseSpritesAddress + (256) * i, ReadBytes(baseSpritesAddress + (256) * index));
					break;
				}
			}
		}


	}
}

public class Sprite
{
	private string name;
    public int Id { get; set; }
    public bool isXFrozen { get; set; }
	public bool isYFrozen { get; set; }

	private long baseAddress;
	private long xPosAddress;
	private long xColAddress;
	private long yPosAddress;
	private long yColAddress;
	private long animAddress;

	private long idAddress;

	private RamAPI ram;

	public Sprite(long _baseAddress)
	{
		baseAddress = _baseAddress;
		ram = RamAPI.GetInstance();

		xPosAddress = baseAddress + 81;
		xColAddress = baseAddress + 71;

		yPosAddress = baseAddress + 89;
		yColAddress = baseAddress + 75;

		idAddress = baseAddress + 239;

		animAddress = baseAddress + 35;

		Id = GetId();
	}

	public void SetXPos(int newPos)
	{
		ram.WriteByte(xPosAddress, newPos);
		ram.WriteByte(xColAddress, newPos);
	}

	public void SetYPos(int newPos)
	{
		ram.WriteByte(yPosAddress, newPos);
		ram.WriteByte(yColAddress, newPos);
	}



	public Vector2 GetPos()
	{
		Vector2 pos = new Vector2
		{
			X = ram.ReadByte(xPosAddress),
			Y = ram.ReadByte(yPosAddress)
		};
		return pos;
	}

	public void SetAnim(int newAnim)
	{
		ram.WriteByte(animAddress, newAnim);
	}

	public int GetAnim()
	{
		int anim = ram.ReadByte(animAddress);
		return anim;
	}

	private int GetId()
	{
		int _id = ram.ReadByte(idAddress);
		return _id;
	}


}
