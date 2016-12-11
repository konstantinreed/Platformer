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
		WallJumping,
		Falling,
		WallFalling,
		Landing
	}

	public struct PlayerState : IStepState
	{
		public bool WasInitialized { get; set; }
		public int Step { get; set; }

		public PlayerAnimation Animation;
		public Vector2 Position;
		public Vector2 Velocity;
		public float Rotation;
		public bool IsGrounded;
		public bool IsClingedWall;
		public int JumpedStep;
		public int LandedStep;
		public int HorizontalCorrectionStep;
		public float LastFallingVelocityYFactor;
		public float LandingVelocityYFactor;

		public bool IsLanded =>
			Animation == PlayerAnimation.Idle ||
			Animation == PlayerAnimation.Running ||
			Animation == PlayerAnimation.Landing;

		public bool IsWalking =>
			Animation == PlayerAnimation.Idle ||
			Animation == PlayerAnimation.Running;

		public bool DoneJumpingConditions => IsGrounded && IsLanded;

		public bool DoneWallJumpingConditions =>
			!IsGrounded &&
			IsClingedWall &&
			Step > JumpedStep + PhysicsPlayer.JumpingSteps;

		public bool DoneJumpingReinforcementConditions =>
			Animation == PlayerAnimation.Jumping &&
			Step == JumpedStep + PhysicsPlayer.JumpingReinforcement;

		public bool DoneHorizontalCorrectionConditions =>
			Step >= HorizontalCorrectionStep;

		public bool DoneFallingConditions => !IsGrounded && !IsClingedWall;

		public bool DoneWallFallingConditions => !IsGrounded && IsClingedWall;

		public bool DoneLandingConditions =>
			IsGrounded &&
			(
				Animation == PlayerAnimation.Falling ||
				Animation == PlayerAnimation.WallFalling ||
				(
					(Animation == PlayerAnimation.Jumping || Animation == PlayerAnimation.WallJumping) &&
					Step > JumpedStep + PhysicsPlayer.JumpingSteps
				)
			);

		public bool DoneLandingFinishConditions =>
			Animation == PlayerAnimation.Landing &&
			Step >= LandedStep + PhysicsPlayer.LandingSteps;
	}

	public class PhysicsPlayer : PhysicsBody
	{
		// Fixtures consts
		private const float BodyX = 0f;
		private const float BodyY = 0.9f;
		private const float BodyWidth = 0.6f;
		private const float BodyHeight = 1.15f;
		private const float FootX = 0f;
		private const float FootY = 0.305f;
		private const float FootRadius = 0.305f;
		private const float GroundSensorX = 0f;
		private const float GroundSensorY = 0f;
		private const float GroundSensorWidth = 0.25f;
		private const float GroundSensorHeight = 0.125f;
		private const float GroundScopeSensorXOffset = 0.11f;
		private const float GroundScopeSensorYFrom = 0.03f;
		private const float GroundScopeSensorYTo = -0.11f;
		private const float LeftWallSensorX = -0.3f;
		private const float LeftWallSensorY = 0.3f;
		private const float LeftWallSensorWidth = 0.125f;
		private const float LeftWallSensorHeight = 0.36f;
		private const float RightWallSensorX = 0.3f;
		private const float RightWallSensorY = 0.3f;
		private const float RightWallSensorWidth = 0.125f;
		private const float RightWallSensorHeight = 0.36f;
		// Physics consts
		private const float Mass = 70f;
		private const float GravityScale = 5.5f;
		private const float GroundGravityScale = 12.1f;
		private const float Friction = 0f;
		private const float Restitution = 0f;
		// Dynamics consts
		public const float MaxHorizontalSpeed = 10f;
		public const float MaxVerticalSpeed = 19f;
		public const float MinVerticalSpeed = -30f;
		private const float HorizontalCorrectionInAir = 0.1f;
		private const float JumpVerticalSpeed = 15f;
		private const float ReinforcementVerticalSpeed = 13f;
		private const float WallJumpVerticalSpeed = 19f;
		private const float WallJumpHorizontalSpeed = 7f;
		private const float MinWallClingedVerticalSpeed = -5f;
		internal const int JumpingSteps = 4;
		internal const int JumpingReinforcement = 4;
		internal const int WallJumpUnalteredSteps = 13;
		internal const int LandingSteps = 1;

		private readonly PlatformSensor groundSensor;
		private readonly PlatformSensor leftWallSensor;
		private readonly PlatformSensor rightWallSensor;

		public ClientInstance Owner { get; set; }
		public InputState Input => Owner.Input;
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

			groundSensor = new PlatformSensor(
				physicsSystem.World,
				new Vector2(GroundSensorWidth, GroundSensorHeight),
				new Vector2(GroundSensorX, GroundSensorY),
				new[] {
					new PlatformSensor.ScopeSensorData {
						From = new Vector2(-GroundScopeSensorXOffset, GroundScopeSensorYFrom),
						To = new Vector2(-GroundScopeSensorXOffset, GroundScopeSensorYTo)
					},
					new PlatformSensor.ScopeSensorData {
						From = new Vector2(GroundScopeSensorXOffset, GroundScopeSensorYFrom),
						To = new Vector2(GroundScopeSensorXOffset, GroundScopeSensorYTo)
					}
				}
			);
			leftWallSensor = new PlatformSensor(
				physicsSystem.World,
				new Vector2(LeftWallSensorWidth, LeftWallSensorHeight),
				new Vector2(LeftWallSensorX, LeftWallSensorY)
			);
			rightWallSensor = new PlatformSensor(
				physicsSystem.World,
				new Vector2(RightWallSensorWidth, RightWallSensorHeight),
				new Vector2(RightWallSensorX, RightWallSensorY)
			);
		}

		public override void FixedUpdate(float delta)
		{
			groundSensor.Update(Body.Position);
			if (!groundSensor.IsActive) {
				leftWallSensor.Update(Body.Position);
				rightWallSensor.Update(Body.Position);
			} else {
				leftWallSensor.Deactivate();
				rightWallSensor.Deactivate();
			}

			State.IsGrounded = groundSensor.IsActive;
			State.Rotation = groundSensor.Radians;
			State.IsClingedWall = leftWallSensor.IsActive || rightWallSensor.IsActive;

			var minVerticalSpeed = State.Animation == PlayerAnimation.WallFalling ? MinWallClingedVerticalSpeed : MinVerticalSpeed;
			var velocityX = Body.LinearVelocity.X;
			var velocityY = Mathf.Clamp(Body.LinearVelocity.Y, minVerticalSpeed, MaxVerticalSpeed);
			
			// Vertical velocity
			if (Input.IsJumpJustPressed) {
				if (State.DoneJumpingConditions) {
					State.Animation = PlayerAnimation.Jumping;
					velocityY = JumpVerticalSpeed;
					State.JumpedStep = State.Step;
					State.HorizontalCorrectionStep = State.JumpedStep;
				} else if (State.DoneWallJumpingConditions) {
					State.Animation = PlayerAnimation.WallJumping;
					velocityY = WallJumpVerticalSpeed;
					var signX = leftWallSensor.IsActive ? 1f : -1f;
					velocityX = signX * WallJumpHorizontalSpeed;
					State.JumpedStep = State.Step;
					State.HorizontalCorrectionStep = State.JumpedStep + WallJumpUnalteredSteps;
				}
			}
			if (velocityY <= 0) {
				if (State.DoneFallingConditions) {
					State.Animation = PlayerAnimation.Falling;
				} else if (State.DoneWallFallingConditions) {
					State.Animation = PlayerAnimation.WallFalling;
				}
			}
			if (State.DoneLandingConditions) {
				State.Animation = PlayerAnimation.Landing;
				State.LandedStep = State.Step;
				State.LandingVelocityYFactor = State.LastFallingVelocityYFactor;
			}
			if (Input.IsJumpPressed && Input.JumpPressedStep == State.JumpedStep && State.DoneJumpingReinforcementConditions) {
				velocityY = ReinforcementVerticalSpeed;
			}
			var velocityYFactor = velocityY / (velocityY >= 0 ? MaxVerticalSpeed : -MinVerticalSpeed);

			// Horizontal velocity
			var inputX = Input.IsLeftPressed != Input.IsRightPressed ? (Input.IsLeftPressed ? -1f : 1f) : 0f;
			if (State.IsLanded) {
				velocityX = inputX * MaxHorizontalSpeed;
			} else if (State.DoneHorizontalCorrectionConditions) {
				velocityX += inputX * MaxHorizontalSpeed * HorizontalCorrectionInAir;
				velocityX = Mathf.Clamp(velocityX, -MaxHorizontalSpeed, MaxHorizontalSpeed);
				if ((leftWallSensor.IsActive && velocityX < 0.01f) || (rightWallSensor.IsActive && velocityX > 0.01f)) {
					velocityX = 0f;
				}
			}
			var velocityXFactor = Mathf.Abs(velocityX) / MaxHorizontalSpeed;
			
			if (State.IsWalking || State.DoneLandingFinishConditions) {
				State.Animation = velocityXFactor > 0.01f ? PlayerAnimation.Running : PlayerAnimation.Idle;
			}

			// Apply physics
			Body.LinearVelocity = new Vector2(velocityX, velocityY);
			Body.GravityScale = State.IsLanded ? GroundGravityScale : GravityScale;
			
			if (velocityYFactor <= -0.01f) {
				State.LastFallingVelocityYFactor = velocityYFactor;
			}
		}
	}
}
