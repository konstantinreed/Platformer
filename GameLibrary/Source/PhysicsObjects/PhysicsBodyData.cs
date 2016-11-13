using System;
using FarseerPhysics.Dynamics.Contacts;

namespace GameLibrary
{
	public enum PhysicsContactSide
	{
		A,
		B
	}

	public class PhysicsBodyData
	{
		public bool IsPlatform;

		public Func<Contact, PhysicsContactSide, bool> BeginContact;
		public Action<Contact, PhysicsContactSide> EndContact;
	}
}
