using LiteNetLib;
using LiteNetLib.Utils;
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

		private const string Connect_Key = "projectHuntConnectionKey";
		private const int tickRate = 20;
		private const int tickTime = 1000 / tickRate;

		public static Dictionary<long, Player> players;

		private static NetManager server;
		private static EventBasedNetListener listener;

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
				//Console.WriteLine("Client {0} latency was updated to {1}", peer.ConnectId, latency);
			};

			listener.PeerConnectedEvent += (peer) =>
			{

				Console.WriteLine("Peer ({0}) connected, waiting for identification.", peer.EndPoint.ToString());
				
			};

			listener.PeerDisconnectedEvent += (peer, reason) =>
			{
				Console.WriteLine("Peer {0} disconnected", peer.EndPoint);
			};

			listener.NetworkReceiveEvent += (peer, data) =>
			{
				byte opCode = data.GetByte();

				if(opCode == 0)	//Identification packet => (int) nameLength; (string) name; (byte[]) uuid
				{
					IdentificationPacket identificationPacket = Packets.GetIdentification(data.GetRemainingBytes());
					Console.WriteLine("Player {0} is logged in with id {1}", identificationPacket.name, identificationPacket.id);

					players.Add(peer.ConnectId, new Player(identificationPacket, peer));
					return;
				}
				
				Player p;
				players.TryGetValue(peer.ConnectId, out p);
				if (p == null)
				{
					server.DisconnectPeer(peer, Encoding.Unicode.GetBytes("Protocol error. Player must sent identification packet."));
					return;
				}

				switch(opCode)
				{
					case 1:     //Chat message packet => (int) messageLength; (string) message; (int -> cast to enum) recipients
						string msg = Packets.GetChatMsg(data.GetRemainingBytes());
						SendToAll(Packets.CreateChat(p.GetName(), msg), SendOptions.ReliableOrdered);
						break;

					case 2:		//Position update packet => (transform/compressed) transform; (int) movedAtTick
						break;
					case 3:		//Shoot command => (Vector3/compressed) direction; (int) shootAtTick
						break;
					case 4:		//TODO: define use
						break;
					case 5:		//TODO: define use
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

				watch.Restart();

					server.PollEvents();
					tickCount++;
					Game.Update();
					//SendSnapshotToAll(TakeSnapshot());
		
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

		public static void SendToAll(NetDataWriter data, SendOptions option)
		{
			foreach (NetPeer peer in server.GetPeers())
				peer.Send(data, option);
		}

	}
}
