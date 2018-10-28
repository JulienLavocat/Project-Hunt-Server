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
		private static long time = 0;	//Time since server has started
		private static bool targetAssignmentRequired = true;
		private static State state = State.WAITTING;

		public static void Update(long deltaTime)
		{
			tick++;
			time += deltaTime;

			switch(state)
			{
				case State.WAITTING:	//In this state, server wait to have a sufisant amount of players connected to start the game



					break;
				case State.STARTED:
					break;
				case State.ENDED:
					break;
			}

			targetAssignmentRequired = (tick * Server.tickTime) % 30 == 0;

			if(targetAssignmentRequired)
			{
				AssignTargets();
			}

		}

		public static void AssignTargets()
		{
			if (Server.players.Count <= 6)
				return;

			Random r = new Random();

			List<Player> players = Server.players.Values.ToList();
			List<Player> left = new List<Player>(players);

			foreach (Player player in players)
			{
				Player newTarget = left.ElementAt(r.Next(left.Count));
				while (newTarget == player)
				{
					newTarget = left.ElementAt(r.Next(left.Count));
				}

				left.Remove(newTarget);
				player.SetTarget(newTarget);

				Console.WriteLine(player.GetName() + " target is now: " + newTarget.GetName());
				player.GetPeer().Send(Packets.CreateTargetChange(newTarget.GetName()), SendOptions.ReliableOrdered);
			}
			targetAssignmentRequired = false;
		}

	}

}
