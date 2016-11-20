using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public enum PlayerAnimation
	{
		Idle,
		Running,
		Jumping,
		Falling,
		Landing
	}

	public struct PlayerState : IStepState
	{
		public bool WasInitialized { get; set; }
		public int Step { get; set; }

		public PlayerAnimation Animation;
		public Vector2 Position;
		public Vector2 Velocity;
		public bool IsGrounded;
		public int JumpedStep;
		public int LandedStep;

		// TODO: Old landing system. Rework.
		public float LastLandingVelocityYFactor;
		public float LandingVelocityYFactor;
	}

	public class PhysicsPlayer : PhysicsBody
	{
		// Fixtures consts
		private const float BodyX = 0f;
		private const float BodyY = 1f;
		private const float BodyWidth = 0.95f;
		private const float BodyHeight = 0.95f;
		private const float FootX = 0f;
		private const float FootY = 0.5f;
		private const float FootRadius = 0.5f;
		private const float GroundSensorX = 0f;
		private const float GroundSensorY = 0f;
		private const float GroundSensorWidth = 0.25f;
		private const float GroundSensorHeight = 0.125f;
		// Physics consts
		private const float Mass = 70f;
		private const float GravityScale = 5.5f;
		private const float GroundGravityScale = 12.1f;
		private const float Friction = 0f;
		private const float Restitution = 0f;
		// Dynamics consts
		public const float MaxHorizontalSpeed = 10f;
		public const float MinVerticalSpeed = -30f;
		public const float MaxVerticalSpeed = 10f;
		private const float NoGravityScaleAfterJumpSteps = 4;
		private const int LandingSteps = 5;

		private PlatformSensor groundSendor;

		public ClientInstance Owner { get; set; }
		public InputState Input { get { return Owner.Input; } }
		public PlayerState State;

		public PhysicsPlayer(PhysicsSystem physicsSystem, Vector2 position)
			: base(physicsSystem, BodyType.Dynamic, position, 0f, true, Mass, GravityScale, Friction, Restitution)
		{
			var bodyPosition = new Vector2(BodyX, BodyY);
			var bodyHalf = new Vector2(BodyWidth, BodyHeight) * 0.5f;
			var bodyShape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					bodyPosition + new Vector2(-bodyHalf.X, -bodyHalf.Y),
					bodyPosition + new Vector2(bodyHalf.X, -bodyHalf.Y),
					bodyPosition + new Vector2(bodyHalf.X, bodyHalf.Y),
					bodyPosition + new Vector2(-bodyHalf.X, bodyHalf.Y)
				})
			};
			Body.CreateFixture(bodyShape);

			var footShape = new CircleShape(FootRadius, 1f) {
				Position = new Vector2(FootX, FootY)
			};
			Body.CreateFixture(footShape);

			groundSendor = new PlatformSensor(
				physicsSystem.World,
				GroundSensorWidth,
				GroundSensorHeight,
				GroundSensorX,
				GroundSensorY
			);
		}

		public override void FixedUpdate(float delta)
		{
			groundSendor.Update(Body.Position);
			State.IsGrounded = groundSendor.IsActive;

			var velocityX = Body.LinearVelocity.X;
			var velocityY = Mathf.Clamp(Body.LinearVelocity.Y, MinVerticalSpeed, MaxVerticalSpeed);
			
			// Vertical velocity
			if (
				Input.IsJumpPressed &&
				State.Animation != PlayerAnimation.Jumping &&
				State.Animation != PlayerAnimation.Falling &&
				State.IsGrounded &&
				State.Step > State.JumpedStep + NoGravityScaleAfterJumpSteps
			) {
				State.Animation = PlayerAnimation.Jumping;
				velocityY = MaxVerticalSpeed;
				State.JumpedStep = State.Step;
			}
			var velocityYFactor = velocityY / (velocityY >= 0 ? MaxVerticalSpeed : -MinVerticalSpeed);
			if (!State.IsGrounded && velocityY <= 0) {
				State.Animation = PlayerAnimation.Falling;
			}
			if (
				State.IsGrounded &&
				(
					(
						State.Animation == PlayerAnimation.Jumping &&
						State.Step > State.JumpedStep + NoGravityScaleAfterJumpSteps
					) ||
					State.Animation == PlayerAnimation.Falling
				)
			) {
				State.Animation = PlayerAnimation.Landing;
				State.LandedStep = State.Step;
				State.LandingVelocityYFactor = State.LastLandingVelocityYFactor;
			}
			var isOnGround =
				State.Animation == PlayerAnimation.Idle ||
				State.Animation == PlayerAnimation.Running ||
				State.Animation == PlayerAnimation.Landing;

			// Horizontal velocity
			var inputX = Input.IsLeftPressed != Input.IsRightPressed ? (Input.IsLeftPressed ? -1f : 1f) : 0f;
			velocityX = inputX * MaxHorizontalSpeed;
			// TODO: Rework player jump
			var velocityXFactor = Mathf.Abs(velocityX) / MaxHorizontalSpeed;
			
			if (State.Animation == PlayerAnimation.Landing && State.Step > State.LandedStep + LandingSteps) {
				State.Animation = velocityXFactor > 0.01f ? PlayerAnimation.Running : PlayerAnimation.Idle;
			}

			// Apply physics
			Body.LinearVelocity = new Vector2(velocityX, velocityY);
			// TODO: Rework player jump
			Body.GravityScale = isOnGround ? GroundGravityScale : GravityScale;

			//
			if (velocityYFactor <= -0.01f) {
				State.LastLandingVelocityYFactor = velocityYFactor;
			}
		}
	}
}
