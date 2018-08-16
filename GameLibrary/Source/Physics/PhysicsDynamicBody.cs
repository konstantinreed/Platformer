using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;

namespace GameLibrary
{
	internal abstract class PhysicsDynamicBody : PhysicsBody
	{
		public PhysicsDynamicBody(
			GamePhysics physics,
			BodyType bodyType = BodyType.Dynamic,
			Vector2 position = default(Vector2),
			float rotation = 0f,
			bool isFixedRotation = false,
			float mass = 20f,
			float gravityScale = 1f,
			float friction = 0.2f,
			float restitution = 0f
		) : base(physics, bodyType, position, rotation, isFixedRotation, mass, gravityScale, friction, restitution)
		{
		}

		public abstract void RewindForward();
		public abstract void RewindBack();
	}

	internal abstract class PhysicsDynamicBody<T> : PhysicsDynamicBody where T : PhysicsBodyState, new()
	{
		private readonly T initialState = new T();

		protected abstract Func<float, T, T, T> StateLerpFunc { get; }

		public readonly RingStepBuffer<T> States = new RingStepBuffer<T>(Settings.SavedStatesCount);
		public T State { get; private set; }

		public PhysicsDynamicBody(
			GamePhysics physics,
			BodyType bodyType = BodyType.Dynamic,
			Vector2 position = default(Vector2),
			float rotation = 0f,
			bool isFixedRotation = false,
			float mass = 20f,
			float gravityScale = 1f,
			float friction = 0.2f,
			float restitution = 0f
		) : base(physics, bodyType, position, rotation, isFixedRotation, mass, gravityScale, friction, restitution)
		{
			State = States[0];
			State.CopyFromBody(Body);
			initialState.Copy(State);
        }

		public sealed override void RewindForward()
		{
			var lastState = State;
            States.RewindForward(States.CurrentStep + 1);
			State = States[States.CurrentStep];
			State.Copy(lastState);
			State.ApplyToBody(Body);
		}

		public sealed override void RewindBack()
		{
			State.Copy(States[States.CurrentStep - 1]);
			State.ApplyToBody(Body);
        }

		public override void Updated(float delta)
		{
			base.Updated(delta);
			State.CopyFromBody(Body);
		}

		public T GetState(float progress)
		{
			return StateLerpFunc(progress, States.CurrentStep > 0 ? States[States.CurrentStep - 1] : State, State);
		}

		public void SetPosition(Vector2 value)
		{
			State.Copy(initialState);
			State.BodyCache.Position = value;
			State.Position = value;
			State.ApplyToBody(Body);
		}
	}
}
