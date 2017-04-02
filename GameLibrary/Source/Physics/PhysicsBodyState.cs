using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public abstract class PhysicsBodyState : IStepState
	{
		internal Body.Cache BodyCache = new Body.Cache();

		public Vector2 Position;
		public Vector2 LinearVelocity;
		public int Step { get; set; }

		public virtual void Reset()
		{
			BodyCache.Reset();
			Position = default(Vector2);
			LinearVelocity = default(Vector2);
		}

		internal virtual void Copy(PhysicsBodyState state)
		{
			BodyCache.Clone(state.BodyCache);
			Position = BodyCache.Position;
			LinearVelocity = BodyCache.LinearVelocity;
		}

		internal void ApplyToBody(Body body)
		{
			body.FromCache(BodyCache);
		}

		internal void CopyFromBody(Body body)
		{
			body.ToCache(BodyCache);
			Position = BodyCache.Position;
			LinearVelocity = BodyCache.LinearVelocity;
		}

		internal static void Lerp(float progress, PhysicsBodyState from, PhysicsBodyState to, PhysicsBodyState progressState)
		{
			progressState.Position = Mathf.Lerp(progress, from.Position, to.Position);
			progressState.LinearVelocity = Mathf.Lerp(progress, from.LinearVelocity, to.LinearVelocity);
		}
	}
}
