using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;

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
		Landing,
		Hitting,
		Dying
	}

	internal enum PlayerPhysicsState
	{
		Landed,
		Jumping,
		WallJumping,
		Falling,
		WallFalling,
		Landing
	}

	public class PlayerState : PhysicsBodyState
	{
		public const float MaxHorizontalSpeed = 10f;
		public const float MaxVerticalSpeed = 19f;
		public const float MinVerticalSpeed = -30f;

		internal PlayerPhysicsState Physics;
		public PlayerAnimation Animation;
		public float ScopeAngle;
		public bool IsGrounded;
		public bool IsClingedWall;
		internal int InputJumpedStep;
		internal int JumpedStep;
		internal int LandedStep;
		internal int HorizontalCorrectionStep;
		internal int HittedStep;
		internal float LastFallingVelocityYFactor;
		public float LandingVelocityYFactor;
		internal bool IsDead;

		internal bool IsNearGround => Physics == PlayerPhysicsState.Landed || Physics == PlayerPhysicsState.Landing;
		internal bool DoneJumpingConditions => IsGrounded && IsNearGround;
		internal bool DoneWallJumpingConditions => !IsGrounded && IsClingedWall && Step > JumpedStep + PhysicsPlayer.JumpingSteps;
		internal bool DoneJumpingReinforcementConditions =>
			Physics == PlayerPhysicsState.Jumping &&
			Step == JumpedStep + PhysicsPlayer.JumpingReinforcement;
		internal bool DoneHorizontalCorrectionConditions => Step >= HorizontalCorrectionStep;
		internal bool DoneFallingConditions => !IsGrounded && !IsClingedWall;
		internal bool DoneWallFallingConditions => !IsGrounded && IsClingedWall;
		internal bool DoneLandingConditions =>
			IsGrounded &&
			(
				Physics == PlayerPhysicsState.Falling ||
				Physics == PlayerPhysicsState.WallFalling ||
				(
					(Physics == PlayerPhysicsState.Jumping || Physics == PlayerPhysicsState.WallJumping) &&
					Step > JumpedStep + PhysicsPlayer.JumpingSteps
				)
			);
		internal bool DoneLandingFinishConditions =>
			Physics == PlayerPhysicsState.Landing &&
			Step >= LandedStep + PhysicsPlayer.LandingSteps;
		internal bool IsHitting => !IsDead && Step < HittedStep + PhysicsPlayer.HittingSteps;

		public override void Reset()
		{
			base.Reset();

			Animation = default(PlayerAnimation);
			Physics = default(PlayerPhysicsState);
            ScopeAngle = default(float);
			IsGrounded = default(bool);
			IsClingedWall = default(bool);
			InputJumpedStep = default(int);
			JumpedStep = default(int);
			LandedStep = default(int);
			HorizontalCorrectionStep = default(int);
			HittedStep = default(int) - PhysicsPlayer.HittingSteps;
			LastFallingVelocityYFactor = default(float);
			LandingVelocityYFactor = default(float);
			IsDead = default(bool);
		}

		internal override void Copy(PhysicsBodyState state)
		{
			base.Copy(state);

			var playerState = (PlayerState)state;
			Animation = playerState.Animation;
			Physics = playerState.Physics;
			ScopeAngle = playerState.ScopeAngle;
			IsGrounded = playerState.IsGrounded;
			IsClingedWall = playerState.IsClingedWall;
			InputJumpedStep = playerState.InputJumpedStep;
			JumpedStep = playerState.JumpedStep;
			LandedStep = playerState.LandedStep;
			HorizontalCorrectionStep = playerState.HorizontalCorrectionStep;
			HittedStep = playerState.HittedStep;
			LastFallingVelocityYFactor = playerState.LastFallingVelocityYFactor;
			LandingVelocityYFactor = playerState.LandingVelocityYFactor;
			IsDead = playerState.IsDead;
        }

		internal static PlayerState Lerp(float progress, PlayerState from, PlayerState to)
		{
			var progressState = new PlayerState() {
				Animation = from.Animation,
				Physics = from.Physics,
				ScopeAngle = Mathf.Lerp(progress, from.ScopeAngle, to.ScopeAngle),
				IsGrounded = from.IsGrounded,
				IsClingedWall = from.IsClingedWall,
				InputJumpedStep = from.InputJumpedStep,
				JumpedStep = from.JumpedStep,
				LandedStep = from.LandedStep,
				HorizontalCorrectionStep = from.HorizontalCorrectionStep,
				HittedStep = from.HittedStep,
				LastFallingVelocityYFactor = from.LastFallingVelocityYFactor,
				LandingVelocityYFactor = from.LandingVelocityYFactor,
				IsDead = from.IsDead
			};
			Lerp(progress, from, to, progressState);
			return progressState;
        }
	}

	internal class PhysicsPlayer : PhysicsDynamicBody<PlayerState>
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
		private const float GroundScopeSensorXOffset = 0.21f;
		private const float GroundScopeSensorYFrom = 0.11f;
		private const float GroundScopeSensorYTo = -0.5f;
		private const float GroundSensorContactFraction = 0.19f;
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
		private const float Friction = 0f;
		private const float Restitution = 0f;
		// Dynamics consts
		private const float HorizontalCorrectionInAir = 0.1f;
		private const float JumpVerticalSpeed = 15f;
		private const float ReinforcementVerticalSpeed = 15f;
		private const float WallJumpVerticalSpeed = 19f;
		private const float WallJumpHorizontalSpeed = 7f;
		private const float MinWallClingedVerticalSpeed = -5f;
		internal const int JumpingSteps = 4;
		internal const int JumpingReinforcement = 6;
		internal const int WallJumpUnalteredSteps = 13;
		internal const int LandingSteps = 1;
		internal const int HittingSteps = 16;
		private const float MinHittingVerticalSpeed = -2.5f;
		private const float MaxHittingVerticalSpeed = 5f;

		private readonly PlatformSensor groundSensor;
		private readonly PlatformSensor leftWallSensor;
		private readonly PlatformSensor rightWallSensor;

		private bool IsInputLeft => !State.IsDead && !State.IsHitting && Input.IsLeftPressed;
		private bool IsInputRight => !State.IsDead && !State.IsHitting && Input.IsRightPressed;
		private bool IsInputJumpJust => !State.IsDead && !State.IsHitting && Input.IsJumpJustPressed;
		private bool IsInputJumpReinforcement =>
			!State.IsDead &&
			!State.IsHitting &&
			Input.IsJumpPressed &&
			State.InputJumpedStep == Input.JumpPressedStep &&
			State.DoneJumpingReinforcementConditions;
		private bool IsInputHit => !State.IsDead && !State.IsHitting && Input.IsHitPressed;
		private bool IsInputSuicide => Input.IsSuicidePressed;

		protected override Func<float, PlayerState, PlayerState, PlayerState> StateLerpFunc => PlayerState.Lerp;

		public Player Owner { get; set; }
		public InputState Input => Owner.Input;

		public PhysicsPlayer(GamePhysics physicsSystem, Vector2 position)
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
						From = new Vector2(0f, GroundScopeSensorYFrom),
						To = new Vector2(0f, GroundScopeSensorYTo)
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

		public override void Update(float delta)
		{
			base.Update(delta);

			groundSensor.Update(Body.Position);
			if (!groundSensor.IsActive) {
				leftWallSensor.Update(Body.Position);
				rightWallSensor.Update(Body.Position);
			} else {
				leftWallSensor.Deactivate();
				rightWallSensor.Deactivate();
			}

			State.IsGrounded = groundSensor.IsActive;
			State.IsClingedWall = leftWallSensor.IsActive || rightWallSensor.IsActive;
			State.ScopeAngle = groundSensor.IsActive ? groundSensor.GetAverageRadians(Mathf.HalfPi) - Mathf.HalfPi : 0f;
			var minimalGroundFraction = groundSensor.IsActive ? groundSensor.GetMinimalFraction(1f) : 1f;

			var velocityX = Body.LinearVelocity.X;
			var velocityY = VelocityYClamp(Body.LinearVelocity.Y);

			// Common
			if (IsInputSuicide) {
				State.IsDead = true;
            }
			if (IsInputHit) {
				State.HittedStep = State.Step;
			}

			// Vertical velocity
			if (IsInputJumpJust) {
				if (State.DoneJumpingConditions) {
					State.Physics = PlayerPhysicsState.Jumping;
					velocityY = JumpVerticalSpeed;
					State.InputJumpedStep = Input.JumpPressedStep;
					State.JumpedStep = State.Step;
					State.HorizontalCorrectionStep = State.JumpedStep;
				} else if (State.DoneWallJumpingConditions) {
					State.Physics = PlayerPhysicsState.WallJumping;
					velocityY = WallJumpVerticalSpeed;
					var signX = leftWallSensor.IsActive ? 1f : -1f;
					velocityX = signX * WallJumpHorizontalSpeed;
					State.JumpedStep = State.Step;
					State.HorizontalCorrectionStep = State.JumpedStep + WallJumpUnalteredSteps;
				}
			}
			if (velocityY <= 0) {
				if (State.DoneFallingConditions) {
					State.Physics = PlayerPhysicsState.Falling;
				} else if (State.DoneWallFallingConditions) {
					State.Physics = PlayerPhysicsState.WallFalling;
				}
			}
			if (State.DoneLandingConditions) {
				State.Physics = PlayerPhysicsState.Landing;
				State.LandedStep = State.Step;
				State.LandingVelocityYFactor = State.LastFallingVelocityYFactor;
			}
			if (IsInputJumpReinforcement) {
				velocityY = ReinforcementVerticalSpeed;
			}
			var requiredGravityResistance = false;
			if (State.IsNearGround && (velocityY > 0 || minimalGroundFraction <= GroundSensorContactFraction)) {
				velocityY = 0f;
				requiredGravityResistance = true;
			}
			velocityY = VelocityYClamp(velocityY);
			var velocityYFactor = velocityY / (velocityY >= 0 ? PlayerState.MaxVerticalSpeed : -PlayerState.MinVerticalSpeed);

			// Horizontal velocity
			var inputX = IsInputLeft != IsInputRight ? (IsInputLeft ? -1 : 1) : 0;
			var rotationX = 0f;
			if (groundSensor.IsActive && inputX != 0) {
				var forwardScope = groundSensor.ScopeSensors[inputX == -1 ? 0 : 2];
				var centralScope = groundSensor.ScopeSensors[1];
				var backwardScope = groundSensor.ScopeSensors[inputX == -1 ? 2 : 0];
				if (forwardScope.IsActive && centralScope.IsActive) {
					if (inputX == -1) {
						rotationX =
							forwardScope.Radians >= centralScope.Radians ?
							(
								forwardScope.Radians > backwardScope.Radians || !backwardScope.IsActive ?
								forwardScope.Radians :
								backwardScope.Radians
							) :
							centralScope.Radians;
					} else {
						rotationX =
							forwardScope.Radians <= centralScope.Radians ?
							(
								forwardScope.Radians < backwardScope.Radians || !backwardScope.IsActive ?
								forwardScope.Radians :
								backwardScope.Radians
							) :
							centralScope.Radians;
					}
				} else if (centralScope.IsActive) {
					rotationX = centralScope.Radians;
				} else if (forwardScope.IsActive) {
					rotationX = forwardScope.Radians;
				} else {
					rotationX = State.ScopeAngle + Mathf.HalfPi;
				}
				rotationX -= Mathf.HalfPi;
			}
			if (State.IsNearGround) {
				velocityX = inputX * PlayerState.MaxHorizontalSpeed;
			} else if (State.DoneHorizontalCorrectionConditions) {
				velocityX += inputX * PlayerState.MaxHorizontalSpeed * HorizontalCorrectionInAir;
				velocityX = Mathf.Clamp(velocityX, -PlayerState.MaxHorizontalSpeed, PlayerState.MaxHorizontalSpeed);
				if ((leftWallSensor.IsActive && velocityX < 0.01f) || (rightWallSensor.IsActive && velocityX > 0.01f)) {
					velocityX = 0f;
				}
			}
			if (State.DoneLandingFinishConditions) {
				State.Physics = PlayerPhysicsState.Landed;
			}

			// Apply physics
			Body.GravityScale = requiredGravityResistance ? 0f : GravityScale;
			var directionX = Vector2.Normalize(new Vector2(Mathf.Cos(rotationX), Mathf.Sin(rotationX)));
			var forceX = directionX * velocityX;
			var forceY = Vector2.UnitY * velocityY;
			Body.LinearVelocity = forceX + forceY;

			if (velocityYFactor <= -0.01f) {
				State.LastFallingVelocityYFactor = velocityYFactor;
			}
		}

		private float VelocityYClamp(float value)
		{
			if (State.Physics == PlayerPhysicsState.WallFalling) {
				value = Mathf.Clamp(value, MinWallClingedVerticalSpeed, float.MaxValue);
			}
			if (State.IsHitting) {
				value = Mathf.Clamp(value, MinHittingVerticalSpeed, MaxHittingVerticalSpeed);
			}
			return Mathf.Clamp(value, PlayerState.MinVerticalSpeed, PlayerState.MaxVerticalSpeed);
		}

		public override void Updated(float delta)
		{
			base.Updated(delta);

			if (State.IsDead) {
				State.Animation = PlayerAnimation.Dying;
				return;
			}

			if (State.IsHitting) {
				State.Animation = PlayerAnimation.Hitting;
				return;
			}

			switch (State.Physics) {
				case PlayerPhysicsState.Landed:
					var velocityXFactor = Mathf.Clamp(Mathf.Abs(Body.LinearVelocity.X) / PlayerState.MaxHorizontalSpeed, 0f, 1f);
					State.Animation = velocityXFactor > 0.01f ? PlayerAnimation.Running : PlayerAnimation.Idle;
					break;
				case PlayerPhysicsState.Jumping:
					State.Animation = PlayerAnimation.Jumping;
					break;
				case PlayerPhysicsState.WallJumping:
					State.Animation = PlayerAnimation.WallJumping;
					break;
				case PlayerPhysicsState.Falling:
					State.Animation = PlayerAnimation.Falling;
					break;
				case PlayerPhysicsState.WallFalling:
					State.Animation = PlayerAnimation.WallFalling;
					break;
				case PlayerPhysicsState.Landing:
					State.Animation = PlayerAnimation.Landing;
					break;
			}
		}
	}
}
