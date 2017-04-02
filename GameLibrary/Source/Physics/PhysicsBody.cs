using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	internal abstract class PhysicsBody
	{
		protected readonly GamePhysics physics;

		public readonly Body Body;

		public PhysicsBody(
			GamePhysics physics,
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
			this.physics = physics;

			physics.Objects.Add(this);

			Body = BodyFactory.CreateBody(physics.World, position, rotation);
			Body.BodyType = bodyType;
			Body.FixedRotation = isFixedRotation;
			Body.Mass = mass;
			Body.GravityScale = gravityScale;
			Body.Friction = friction;
			Body.Restitution = restitution;
		}

		public virtual void Update(float delta) { }

		public virtual void Updated(float delta) { }

		public void Unlink()
		{
			physics.Objects.Remove(this);
			physics.World.RemoveBody(Body);
		}
	}
}
