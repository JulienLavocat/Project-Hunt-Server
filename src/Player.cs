using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Hunt
{
	public class Player
	{

		public bool includeInSnapshot;

		private string name;
		private Guid id;
		private Player target;
		private short score;
		private short posX;
		private short posY;
		private short posZ;
		private short rotX;
		private short rotY;
		private short rotZ;
		private float health;
		private NetPeer peer;

		public Player(IdentificationPacket p, NetPeer peer)
		{
			name = p.name;
			id = p.id;
			health = 100.0f;
			includeInSnapshot = true;
			this.peer = peer;
		}

		public byte[] Snapshot()
		{
			NetDataWriter w = new NetDataWriter();

			w.Put(peer.Id);	//4
			w.Put(score);   //2
			w.Put(posX);	//2
			w.Put(posY);    //2
			w.Put(posZ);    //2
			w.Put(rotX);    //2
			w.Put(rotY);    //2
			w.Put(rotZ);    //2
			w.Put(health);  //4

			return w.CopyData();	//Total: 22bytes
		}
		public byte[] TakeFullSnapshot()
		{
			NetDataWriter w = new NetDataWriter();

			w.Put(peer.Id);     //4
			w.Put(name.Length); //4
			w.Put(name);        //Variable
			w.Put(score);       //2
			w.Put(posX);        //2
			w.Put(posY);        //2
			w.Put(posZ);        //2
			w.Put(rotX);        //2
			w.Put(rotY);        //2
			w.Put(rotZ);        //2
			w.Put(health);      //4

			return w.CopyData();
		}

		public void Move(short x, short y, short z)
		{
			posX = x;
			posY = y;
			posZ = z;
			includeInSnapshot = true;
		}
		public void Rotate(short x, short y, short z)
		{
			rotX = x;
			rotY = y;
			rotZ = z;
			includeInSnapshot = true;
		}

		public string GetName()
		{
			return name;
		}
		public Guid GetID()
		{
			return id;
		}
		public Player GetTarget()
		{
			return target;
		}
		public void SetTarget(Player p)
		{
			target = p;
		}
		public float GetHealth()
		{
			return health;
		}
		public void SetHealth(float health)
		{
			this.health = health;
			includeInSnapshot = true;
		}
		public bool TakeDamages(float amount)
		{
			health -= amount;
			includeInSnapshot = true;
			return health <= 0;
		}
		public NetPeer GetPeer()
		{
			return peer;
		}
		public short GetScore()
		{
			return score;
		}
		public void SetScore(short score)
		{
			this.score = score;
			includeInSnapshot = true;
		}
		public void ScoreTargetKill()
		{
			score += 5;
			includeInSnapshot = true;
		}
		public void ScoreRegularKill()
		{
			score += 1;
			includeInSnapshot = true;
		}
		public void ScoreHunterKill()
		{
			score += 3;
			includeInSnapshot = true;
		}
		public short GetPosX()
		{
			return posX;
		}
		public short GetPosY()
		{
			return posY;
		}
		public short GetPosZ()
		{
			return posZ;
		}
		public short GetRotX()
		{
			return rotX;
		}
		public short GetRotY()
		{
			return rotY;
		}
		public short GetRotZ()
		{
			return rotZ;
		}
	}
}