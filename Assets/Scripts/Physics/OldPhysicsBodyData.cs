using System;
using FarseerPhysics.Dynamics.Contacts;

namespace Scripts.Physics
{
	public enum OldPhysicsContactSide
	{
		A,
		B
	}

	public class OldPhysicsBodyData
	{
		public bool IsGround;

		public Func<Contact, OldPhysicsContactSide, bool> BeginContact;
		public Action<Contact, OldPhysicsContactSide> EndContact;
	}
}
