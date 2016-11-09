﻿using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using UnityEngine;

namespace Scripts.Physics
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PhysicsWorld
	{
		private const float SimulationStep = 0.025f;
		private readonly Vec2 gravityForce = new Vec2(0f, -9.82f);

		private float remainDeltaTime;
		private readonly World world;

		public PhysicsWorld()
		{
			world = new World(gravityForce);
			world.ContactManager.BeginContact += BeginContact;
			world.ContactManager.EndContact += EndContact;
		}

		public Body CreateBody(Vector2 position)
		{
			return new Body(world, new Vec2(position.x, position.y));
		}

		public void Update(float deltaTime)
		{
			remainDeltaTime += deltaTime;

			while (remainDeltaTime >= SimulationStep) {
				world.Step(SimulationStep);
				remainDeltaTime -= SimulationStep;
			}
		}

		private static bool BeginContact(Contact contact)
		{
			var result = true;

			var userDataA = contact.FixtureA.UserData as PhysicsBodyData;
			if (userDataA != null && userDataA.BeginContact != null) {
				result = userDataA.BeginContact(contact, PhysicsContactSide.A);
			}

			var userDataB = contact.FixtureB.UserData as PhysicsBodyData;
			if (userDataB != null && userDataB.BeginContact != null) {
				result = result && userDataB.BeginContact(contact, PhysicsContactSide.B);
			}

			return result;
		}

		private static void EndContact(Contact contact)
		{
			var userDataA = contact.FixtureA.UserData as PhysicsBodyData;
			if (userDataA != null && userDataA.EndContact != null) {
				userDataA.EndContact(contact, PhysicsContactSide.A);
			}

			var userDataB = contact.FixtureB.UserData as PhysicsBodyData;
			if (userDataB != null && userDataB.EndContact != null) {
				userDataB.EndContact(contact, PhysicsContactSide.B);
			}
		}
	}
}
