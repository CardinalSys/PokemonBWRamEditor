using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory;

namespace PkmBWRamEditor
{
	internal class RamAPI
	{

		private static RamAPI _instance;

        public string ProcName { get; set; }
        private string _baseSpritesAddress = "?? ?? 3? 02 90 21 25 02 ?? 5? 1D 02 ?? 86 1D 02";
		private Mem _procMem = new Mem();

		public List<byte[]> spritesMemoryLocation = new List<byte[]>();

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
				IEnumerable<long> AoBScanResults = await _procMem.AoBScan(_baseSpritesAddress, true, true, true);

				UpdateSpriteList(AoBScanResults.Last().ToString());

				return AoBScanResults.Last().ToString("X");
			}

			return "Null";
		}

		public void UpdateSpriteList(string lastAddress)
		{
			long address = long.Parse(lastAddress);

			while(_procMem.ReadByte(address.ToString("X")) != 0xFF)
			{
				spritesMemoryLocation.Add(_procMem.ReadBytes((address - 144).ToString("X"), 256));

				address -= 256;
			}

			spritesMemoryLocation.RemoveAt(spritesMemoryLocation.Count - 1);
		}

		public int ReadPos(string address)
		{
			return _procMem.ReadByte(address);
		}
	}
}
