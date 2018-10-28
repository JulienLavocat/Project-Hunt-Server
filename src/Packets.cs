using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt
{
	class Packets
	{

		public static NetDataWriter CreateChat(string from, string msg)
		{
			NetDataWriter w = new NetDataWriter();
			w.Put((byte)0);
			w.Put(from.Length);
			w.Put(from);
			w.Put(msg);
			return w;
		}

		public static NetDataWriter CreateTargetChange(string target)
		{
			NetDataWriter w = new NetDataWriter();
			w.Put((byte)1);
			w.Put(target);
			return w;
		}


		public static IdentificationPacket GetIdentification(byte[] data)
		{
			IdentificationPacket p = new IdentificationPacket();
			NetDataReader r = new NetDataReader(data);
			p.name = r.GetString(r.GetInt());
			p.id = new Guid(r.GetRemainingBytes());
			return p;
		}

		public static string GetChatMsg(byte[] data)
		{
			NetDataReader r = new NetDataReader(data);
			return r.GetString();
		}

		public static NetDataWriter CreateInit()
		{
			NetDataWriter w = new NetDataWriter();

			w.Put((byte)2);

			foreach (Player p in Server.players.Values)
			{
				w.Put(p.TakeFullSnapshot());
			}
			
			return w;
		}

		public static byte[] CreateSnapshot()
		{
			NetDataWriter w = new NetDataWriter();
			w.Put((byte)3);
			foreach (Player p in Server.players.Values)
			{
				if (p.includeInSnapshot != true)
					continue;
				w.Put(p.Snapshot());
				p.includeInSnapshot = false;
			}

			return w.CopyData();
		}

	}
}
