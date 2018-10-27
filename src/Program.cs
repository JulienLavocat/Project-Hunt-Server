using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt
{
	class Program
	{
		static void Main(string[] args)
		{

			Server.Start();
			Console.WriteLine("Project Hunt server started.");
			Server.Run();

		}
	}
}
