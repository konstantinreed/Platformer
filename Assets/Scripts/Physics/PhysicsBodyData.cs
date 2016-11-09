using System;
using FarseerPhysics.Dynamics.Contacts;

namespace Scripts.Physics
{
	public enum PhysicsContactSide
	{
		A,
		B
	}

	public class PhysicsBodyData
	{
		public bool IsGround;

		public Func<Contact, PhysicsContactSide, bool> BeginContact;
		public Action<Contact, PhysicsContactSide> EndContact;
	}
}
