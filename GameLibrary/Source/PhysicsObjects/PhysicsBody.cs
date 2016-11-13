using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public abstract class PhysicsBody
	{
		protected readonly PhysicsSystem physicsSystem;

		public readonly Body Body;

		public PhysicsBody(
			PhysicsSystem physicsSystem,
			BodyType bodyType = BodyType.Static,
			Vector2 position = default(Vector2),
			float rotation = 0f,
			bool isFixedRotation = false,
			float mass = 20f,
			float gravityScale = 1f,
			float friction = 0.2f,
			float restitution = 0f
		)
		{
			this.physicsSystem = physicsSystem;

			physicsSystem.Objects.Add(this);

			Body = BodyFactory.CreateBody(physicsSystem.World, position, rotation);
			Body.BodyType = bodyType;
			Body.FixedRotation = isFixedRotation;
			Body.Mass = mass;
			Body.GravityScale = gravityScale;
			Body.Friction = friction;
			Body.Restitution = restitution;
		}

		public virtual void FixedUpdate(float delta) {}

		public virtual void Update(float delta) {}

		public void Unlink()
		{
			physicsSystem.Objects.Remove(this);
			physicsSystem.World.RemoveBody(Body);
		}
	}
}
