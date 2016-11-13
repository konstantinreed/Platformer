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
		//Jumping, // TODO: Rework player jump
		//Falling,
		//Landing
	}

	public struct PlayerState
	{
		public Vector2 Position;
		public Vector2 Velocity;
		public PlayerAnimation Animation;
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
		private const float NoGravityScaleAfterJumpDuration = 0.2f;
		private const float LandingDuration = 0.25f;

		public ClientInstance Owner { get; set; }
		public InputState Input { get { return Owner.Input; } }
		public PlayerState State;

		//private int groundContactsNum; // TODO: Rework player jump
		//private float jumpedTime = NoGravityScaleAfterJumpDuration + Mathf.ZeroTolerance;
		//private float landedTime = LandingDuration + Mathf.ZeroTolerance;
		//private float lastLandingVelocityYFactor;
		//private float landingVelocityYFactor;

		//public bool IsGrounded { get { return groundContactsNum > 0; } } // TODO: Rework player jump

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

			/* // TODO: Rework player jump
			var groundSensorPosition = new Vector2(GroundSensorX, GroundSensorY);
			var groundSensorHalf = new Vector2(GroundSensorWidth, GroundSensorHeight) * 0.5f;
			var groundSensorShape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					groundSensorPosition + new Vector2(-groundSensorHalf.X, -groundSensorHalf.Y),
					groundSensorPosition + new Vector2(groundSensorHalf.X, -groundSensorHalf.Y),
					groundSensorPosition + new Vector2(groundSensorHalf.X, groundSensorHalf.Y),
					groundSensorPosition + new Vector2(-groundSensorHalf.X, groundSensorHalf.Y)
				})
			};
			var groundSensorFixture = Body.CreateFixture(groundSensorShape);
			groundSensorFixture.IsSensor = true;
			groundSensorFixture.UserData = new PhysicsBodyData() {
				BeginContact = BeginGroundContact,
				EndContact = EndGroundContact
			};
			*/
		}

		public override void FixedUpdate(float delta)
		{
			var velocityX = Body.LinearVelocity.X;
			var velocityY = Mathf.Clamp(Body.LinearVelocity.Y, MinVerticalSpeed, MaxVerticalSpeed);

			/* // TODO: Rework player jump
			// Vertical velocity
			if (CanJump() && IsKeyJumpDown) {
				State = PlayerState.Jumping;
				velocityY = MaxVerticalSpeed;
				jumpedTime = 0f;
			}
			var velocityYFactor = velocityY / (velocityY >= 0 ? MaxVerticalSpeed : -MinVerticalSpeed);
			if (!IsGrounded && velocityY <= 0) {
				State = PlayerState.Falling;
			}
			if (
				IsGrounded &&
				(
					(State == PlayerState.Jumping && jumpedTime >= NoGravityScaleAfterJumpDuration) ||
					State == PlayerState.Falling
				)
			) {
				State = PlayerState.Landing;
				landedTime = 0f;
				landingVelocityYFactor = lastLandingVelocityYFactor;
			}
			var isOnGround = State == PlayerState.Idle || State == PlayerState.Running || State == PlayerState.Landing;
			*/

			// Horizontal velocity
			var inputX = Input.IsLeftPressed != Input.IsRightPressed ? (Input.IsLeftPressed ? -1f : 1f) : 0f;
			velocityX = inputX * MaxHorizontalSpeed;
			/* // TODO: Rework player jump
			var velocityXFactor = Mathf.Abs(velocityX) / MaxHorizontalSpeed;
			
			if (State == PlayerState.Landing && landedTime >= LandingDuration) {
				State = velocityXFactor > 0.01f ? PlayerState.Running : PlayerState.Idle;
			}
			*/

			// Apply physics
			Body.LinearVelocity = new Vector2(velocityX, velocityY);
			/* // TODO: Rework player jump
			Body.GravityScale = isOnGround ? GroundGravityScale : GravityScale;

			//
			if (velocityYFactor <= -0.01f) {
				lastLandingVelocityYFactor = velocityYFactor;
			}
			*/
		}

		/* // TODO: Rework player jump
		private bool CanJump()
		{
			return
				State != PlayerState.Jumping &&
				State != PlayerState.Falling &&
				IsGrounded &&
				jumpedTime >= NoGravityScaleAfterJumpDuration;
		}
		
		private bool BeginGroundContact(Contact contact, PhysicsContactSide side)
		{
			var fixture = side == PhysicsContactSide.A ? contact.FixtureB : contact.FixtureA;
			var userData = fixture.UserData as PhysicsBodyData;

			if (userData != null && userData.IsPlatform) {
				groundContactsNum++;
			}
			return true;
		}

		private void EndGroundContact(Contact contact, PhysicsContactSide side)
		{
			var fixture = side == PhysicsContactSide.A ? contact.FixtureB : contact.FixtureA;
			var userData = fixture.UserData as PhysicsBodyData;

			if (userData != null && userData.IsPlatform) {
				groundContactsNum--;
			}
		}
		*/
	}
}
