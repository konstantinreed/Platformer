using FarseerPhysics.Dynamics;
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
	}
}
