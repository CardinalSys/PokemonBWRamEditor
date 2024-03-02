using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Memory;

namespace PkmBWRamEditor
{
	internal class RamAPI
	{

		private static RamAPI _instance;

        public string ProcName { get; set; }
        private string _baseSpritesAddress = "18 F8 24 02 D8 23 28 02 94 B4 28 02 CC 57 27 02 66 AF 28 02 AC 62 25 02 44 55 00 00 1C 40 00 00 64 21 25 02 04 62 25 02 03 00 4E 19 66 6C 64 6D 6D 64 6C 2E 63";
		private Mem _procMem = new Mem();

		public List<byte[]> spritesMemoryLocationList = new List<byte[]>();

		public IEnumerable<long> AoBScanResults;

		public static RamAPI GetInstance()
		{
			if (_instance == null)
				_instance = new RamAPI();

			return _instance;
		}

		public async Task<string> GetBaseSpriteAddress() 
		{
			ProcName = "DeSmuME_0.9.13_x64.exe";
			if (_procMem.OpenProcess(ProcName))
			{
				AoBScanResults = await _procMem.AoBScan(_baseSpritesAddress, true, true, true);

				spritesMemoryLocationList = CreateSpriteList((AoBScanResults.Last() + 200).ToString());

				return AoBScanResults.Last().ToString("X");
			}

			return "Null";
		}

		public List<byte[]> CreateSpriteList(string lastAddress)
		{
			long address = long.Parse(lastAddress);

			List<byte[]> newList = new List<byte[]>(); ;

			while(_procMem.ReadByte(address.ToString("X")) != 0xFF)
			{
				newList.Add(_procMem.ReadBytes((address - 144).ToString("X"), 256));

				address += 256;
			}

			newList.RemoveAt(newList.Count - 1);

			return newList;
		}

		public void FreezeXPos()
		{
			_procMem.WriteMemory("DeSmuME_0.9.13_x64.exe+2336CF6", "bytes", "90 90 90 90 90 90 90 90");
		}

		public void UnFreezeXPos()
		{
			_procMem.WriteMemory("DeSmuME_0.9.13_x64.exe+2336CF6", "bytes", "46 89 84 09 00 54 91 0A");
		}

		public void FreezeYPos()
		{
			_procMem.WriteMemory("DeSmuME_0.9.13_x64.exe + 233993D", "bytes", "90 90 90 90 90 90 90 90");
		}

		public void UnFreezeYPos()
		{
			_procMem.WriteMemory("DeSmuME_0.9.13_x64.exe + 233993D", "bytes", "46 89 8C 31 00 54 91 0A");
		}

		public int ReadPos(string address)
		{
			return _procMem.ReadByte(address);
		}

		public void WriteRam(int offset, byte value)
		{
			long last = AoBScanResults.Last() - 144;
			string address = (last + offset).ToString("X");
			_procMem.WriteMemory(address, "byte", value.ToString("X"));
		}
	}
}
