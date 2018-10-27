using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hunt
{

	class Server
	{

		private const string Connect_Key = "porjectHuntConnectionKey";
		private const int tickRate = 20;
		private const int tickTime = 1000 / tickRate;

		private static NetManager server;
		private static EventBasedNetListener listener;
		private static Dictionary<long, Player> players; 

		public static void Start()
		{

			listener = new EventBasedNetListener();
			server = new NetManager(listener, 12, Connect_Key);
			players = new Dictionary<long, Player>();

			SetupListeners();

			server.Start(9090);

		}

		private static void SetupListeners()
		{
			listener.NetworkErrorEvent += (endpoint, error) =>
			{
				Console.WriteLine("An error occured on endpoint {0} with error {1}", endpoint.ToString(), error.ToString());
			};

			listener.NetworkLatencyUpdateEvent += (peer, latency) =>
			{
				Console.WriteLine("Client {0} latency was updated to {1}", peer.ConnectId, latency);
			};

			listener.PeerConnectedEvent += (peer) =>
			{
				


			};

			listener.PeerDisconnectedEvent += (peer, reason) =>
			{

			};

			listener.NetworkReceiveEvent += (peer, data) =>
			{

				switch(data.GetByte())
				{
					case 0:		//Chat message packet => (int) messageLength; (string) message; (int -> cast to enum) recipients
						break;
					case 1:		//Position update packet => (transform/compressed) transform; (int) movedAtTick
						break;
					case 2:		//Shoot command => (Vector3/compressed) direction; (int) shootAtTick
						break;
					case 3:		//TODO: define use
						break;
					case 4:		//TODO: define use
						break;
				}

			};

		}

		public static void Run()
		{
			Console.WriteLine("Server running at {0}hz", tickRate);

			Stopwatch watch = new Stopwatch();

			int tickCount = 0;

			while(true) {

				watch.Reset();

					server.PollEvents();
					tickCount++;
					Game.Update();
					SendSnapshotToAll(TakeSnapshot());
		
				watch.Stop();

				int delta = tickTime - (int)watch.ElapsedMilliseconds;
				
				if (delta <= 0)
					continue;
				Thread.Sleep(delta);
			}

		}

		private static byte[] TakeSnapshot()
		{
			return new byte[] { 0 };
		}

		private static void SendSnapshotToAll(byte[] data)
		{
			foreach(NetPeer peer in server.GetPeers())
				peer.Send(data, SendOptions.Sequenced);
		}

		private static void SendReliableOrderedToAll(byte[] data)
		{
			foreach (NetPeer peer in server.GetPeers())
				peer.Send(data, SendOptions.ReliableOrdered);
		}

	}
}
