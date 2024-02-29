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
        private string _baseSpritesAddress = "?? ?? 3? 02 90 21 25 02 ?? 5? 1D 02 ?? 86 1D 02";
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

				spritesMemoryLocationList = CreateSpriteList(AoBScanResults.Last().ToString());

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

				address -= 256;
			}

			newList.RemoveAt(newList.Count - 1);

			return newList;
		}



		public int ReadPos(string address)
		{
			return _procMem.ReadByte(address);
		}

		public void WriteRam(int offset, byte value)
		{
			long last = AoBScanResults.Last();
			string address = (last - offset).ToString("X");
			_procMem.WriteMemory(address, "byte", value.ToString("X"));
		}
	}
}
