using System;

namespace Hunt
{
	public class Player
	{

		private string name;
		private Guid id;
		private Player target;
		private float health;

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
		}
		public bool TakeDamages(float amount)
		{
			health -= amount;
			return health <= 0;
		}

	}
}