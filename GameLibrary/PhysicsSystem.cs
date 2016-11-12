using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class PhysicsSystem
	{
		private const float SimulationStep = 0.025f;
		private readonly Vector2 gravityForce = new Vector2(0f, -9.82f);

		private float remainDeltaTime;

		public World World { get; private set; }
		public readonly List<PhysicsBody> Objects = new List<PhysicsBody>();

		public PhysicsSystem()
		{
			World = new World(gravityForce);
		}

		public void FixedUpdate()
		{
			World.Step(SimulationStep);

			foreach (var physicsObject in Objects) {
				physicsObject.FixedUpdate();
			}
		}

		public void Update(float delta)
		{
			remainDeltaTime += delta;

			while (remainDeltaTime >= SimulationStep) {
				FixedUpdate();
				remainDeltaTime -= SimulationStep;
			}

			foreach (var physicsObject in Objects) {
				physicsObject.Update(delta);
			}
		}
	}
}
