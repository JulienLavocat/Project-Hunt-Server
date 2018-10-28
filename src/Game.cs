using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hunt
{
	class Game
	{

		private static int tick = 0;
		private static int lastPlayerCount = 0;
		private static long time = 0;   //Time since server has started
		private static long targetAssignedSince;
		private static bool targetNeverAssigned = true;
		private static State state = State.WAITTING;

		public static void Update(long deltaTime)
		{
			tick++;
			time += deltaTime;

			switch(state)
			{
				case State.WAITTING:    //In this state, server wait to have a sufisant amount of players connected to start the game
					UpdateWaittingState(deltaTime);
					break;
				case State.STARTED:
					UpdateStartedState(deltaTime);
					break;
				case State.ENDED:
					UpdateEndedState(deltaTime);
					break;
			}

			Thread.Sleep(10);

		}

		private static void UpdateWaittingState(long deltaTime)
		{
			if (Server.players.Count >= 6)
				state = State.STARTED;


			//TODO Setup players for game start

		}

		private static void UpdateStartedState(long deltaTime)
		{
			if (IsTargetAssignmentRequired(deltaTime))
				AssignTargets();
		}

		private static void UpdateEndedState(long deltaTime)
		{
			
		}

		private static bool IsTargetAssignmentRequired(long deltaTime)
		{
			targetAssignedSince += deltaTime;
			return targetAssignedSince >= 10000 || targetNeverAssigned;	//Target re-assigned every 30 seconds;
		}

		private static void AssignTargets()
		{
			targetNeverAssigned = false;
			targetAssignedSince = 0;
			Log("Reassigning targets at time: " + time);
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

				player.GetPeer().Send(Packets.CreateTargetChange(newTarget.GetName()), DeliveryMethod.ReliableOrdered);
			}
		}

		private static void Log(string msg)
		{
			Console.WriteLine("[Game] " + msg);
		}
	}

}
