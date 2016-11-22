﻿using FarseerPhysics.Collision.Shapes;
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
		public float LastFallingVelocityYFactor;
		public float LandingVelocityYFactor;

		public bool DoneJumpingConditions
		{
			get { return IsGrounded && Animation != PlayerAnimation.Jumping && Animation != PlayerAnimation.Falling; }
		}
		public bool DoneFallingConditions { get { return !IsGrounded; } }
		public bool DoneLandingConditions
		{
			get
			{
				return
					IsGrounded &&
					(
						Animation == PlayerAnimation.Falling ||
						(
							Animation == PlayerAnimation.Jumping &&
							Step > JumpedStep + PhysicsPlayer.JumpingSteps
						)
					);
			}
		}
		public bool IsLanded
		{
			get
			{
				return
					Animation == PlayerAnimation.Idle ||
					Animation == PlayerAnimation.Running ||
					Animation == PlayerAnimation.Landing;
			}
		}
		public bool DoneLandingFinishConditions
		{
			get { return Animation == PlayerAnimation.Landing && Step >= LandedStep + PhysicsPlayer.LandingSteps; }
		}
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
		private const float HorizontalCorrectionInAir = 0.1f;
		public const float MinVerticalSpeed = -30f;
		public const float MaxVerticalSpeed = 20f;
		internal const float JumpingSteps = 4;
		internal const int LandingSteps = 1;

		private readonly PlatformSensor groundSendor;

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
			if (Input.IsJumpPressed && State.DoneJumpingConditions) {
				State.Animation = PlayerAnimation.Jumping;
				velocityY = MaxVerticalSpeed;
				State.JumpedStep = State.Step;
			}
			if (velocityY <= 0 && State.DoneFallingConditions) {
				State.Animation = PlayerAnimation.Falling;
			}
			if (State.DoneLandingConditions) {
				State.Animation = PlayerAnimation.Landing;
				State.LandedStep = State.Step;
				State.LandingVelocityYFactor = State.LastFallingVelocityYFactor;
			}
			var velocityYFactor = velocityY / (velocityY >= 0 ? MaxVerticalSpeed : -MinVerticalSpeed);

			// Horizontal velocity
			var inputX = Input.IsLeftPressed != Input.IsRightPressed ? (Input.IsLeftPressed ? -1f : 1f) : 0f;
			if (State.IsLanded) {
				velocityX = inputX * MaxHorizontalSpeed;
			} else {
				velocityX += inputX * MaxHorizontalSpeed * HorizontalCorrectionInAir;
				velocityX = Mathf.Clamp(velocityX, -MaxHorizontalSpeed, MaxHorizontalSpeed);
			}
			var velocityXFactor = Mathf.Abs(velocityX) / MaxHorizontalSpeed;
			
			if (State.DoneLandingFinishConditions) {
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
