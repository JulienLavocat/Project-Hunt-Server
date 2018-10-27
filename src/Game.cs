using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt
{
	class Game
	{

		private static int tick = 0;
		private static bool targetAssignmentRequired = true;

		public static void Update()
		{
			tick++;

			if(targetAssignmentRequired)
			{
				AssignTarget();
			}

		}

		public static void AssignTarget()
		{
			if (Server.players.Count <= 1)
				return;

			Random r = new Random();

			foreach(Player p in Server.players.Values)
			{
				Player newTarget = Server.players.ElementAt(r.Next(Server.players.Count)).Value;
				p.SetTarget(newTarget);
				Console.WriteLine(p.GetName() + " target is now: " + newTarget.GetName());
				p.GetPeer().Send(Packets.CreateTargetChange(newTarget.GetName()), SendOptions.ReliableOrdered);
			}
			targetAssignmentRequired = false;
		}

		private static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
		{
			Random rand = new Random();
			List<TValue> values = Enumerable.ToList(dict.Values);
			int size = dict.Count;
			while (true)
			{
				yield return values[rand.Next(size)];
			}
		}

	}
}
