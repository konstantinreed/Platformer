using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public abstract class PhysicsBody
	{
		private float gravityScaleStandart;
		private float gravityScaleMultiplier;

		protected readonly GameApplication application;

		public readonly Body Body;

		public float GravityScaleMultiplier
		{
			get { return gravityScaleMultiplier; }
			set
			{
				Body.GravityScale = gravityScaleStandart * gravityScaleMultiplier;
				gravityScaleMultiplier = value;
			}
		}

		public PhysicsBody(
			GameApplication application,
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
			this.application = application;

			application.PhysicsSystem.Objects.Add(this);

			Body = BodyFactory.CreateBody(application.PhysicsSystem.World, position, rotation);
			Body.BodyType = bodyType;
			Body.FixedRotation = isFixedRotation;
			Body.Mass = mass;
			Body.GravityScale = gravityScale;
			Body.Friction = friction;
			Body.Restitution = restitution;

			gravityScaleStandart = Body.GravityScale;
		}

		public virtual void FixedUpdate() {}

		public virtual void Update(float delta) {}

		public virtual void Unlink()
		{
			application.PhysicsSystem.Objects.Remove(this);
			application.PhysicsSystem.World.RemoveBody(Body);
		}
	}
}
