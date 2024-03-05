using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkmBWRamEditor
{
	internal class Server
	{
		private static Server _instance;

		public static Server GetInstance()
		{
			if (_instance == null)
				_instance = new Server();

			return _instance;
		}

	}
}
