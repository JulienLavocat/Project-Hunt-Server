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
		public const int tickTime = 1000 / tickRate;

		private const int tickRate = 1;

		public static Dictionary<long, Player> players;

		private static NetManager server;
		private static EventBasedNetListener listener;

		public static void Start()
		{

			listener = new EventBasedNetListener();
			server = new NetManager(listener);
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
				//Console.WriteLine("Client {0} latency was updated to {1}", peer.Id, latency);
			};

			listener.ConnectionRequestEvent += rq =>
			{
				NetPeer peer;
				if (server.PeersCount <= 14 /* max connections */)
					peer = rq.Accept();
				else
				{
					rq.Reject();
					return;
				}
					
				IdentificationPacket p = Packets.GetIdentification(rq.Data.GetRemainingBytes());
				players.Add(peer.Id, new Player(p, peer));
				Log("Player " + p.name + " is logged in with id " + p.id);
			};

			listener.PeerConnectedEvent += (peer) =>
			{
				peer.Send(Packets.CreateInit(), DeliveryMethod.ReliableOrdered);
			};

			listener.PeerDisconnectedEvent += (peer, reason) =>
			{
				Log("Peer " + peer.EndPoint + " disconnected");
				if (players.ContainsKey(peer.Id))
					players.Remove(peer.Id);
			};

			listener.NetworkReceiveEvent += (peer, data, method) =>
			{	
				Player p;
				players.TryGetValue(peer.Id, out p);
				if (p == null)
				{
					server.DisconnectPeer(peer, Encoding.Unicode.GetBytes("Protocol error. Player must sent identification packet."));
					return;
				}

				switch(data.GetByte())
				{
					case 1:     //Chat message packet => (int) messageLength; (string) message; (int -> cast to enum) recipients
						string msg = Packets.GetChatMsg(data.GetRemainingBytes());
						SendToAll(Packets.CreateChat(p.GetName(), msg), DeliveryMethod.ReliableOrdered);
						break;

					case 2:     //Position update packet => (transform/compressed) transform; (int) movedAtTick

						p.Move(data.GetShort(), data.GetShort(), data.GetShort());
						p.Rotate(data.GetShort(), data.GetShort(), data.GetShort());

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
			Log("Running at " + tickRate + "hz");

			Stopwatch watch = new Stopwatch();

			int delta = 0;

			while(true) {

				watch.Restart();

					server.PollEvents();
					Game.Update(delta);
					SendSnapshotToAll(Packets.CreateSnapshot());
				
				watch.Stop();

				delta = tickTime - (int)watch.ElapsedMilliseconds;

				if (delta <= 0)
					continue;
				Thread.Sleep(delta);
			}

		}

		public static void SendToAll(NetDataWriter data, DeliveryMethod option)
		{
			foreach (NetPeer peer in server.GetPeers(ConnectionState.Connected))
				peer.Send(data, option);
		}

		public static void SendSnapshotToAll(byte[] data)
		{
			if (data.Length == 1)
				return;
			
			foreach (NetPeer peer in server.GetPeers(ConnectionState.Connected))
				peer.Send(data, DeliveryMethod.ReliableSequenced);
		}

		private static void Log(String msg)
		{
			Console.WriteLine("[Server] " + msg);
		}

	}
}
