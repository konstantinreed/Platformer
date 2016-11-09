using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using Scripts.Physics;
using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PhysicsPlayer : PhysicsBody
	{
		private int groundContactsNum;

		public Vector2 BodyPosition;
		public Vector2 BodySize;
		public Vector2 FootPosition;
		public float FootRadius;
		public Vector2 GroundSensorPosition;
		public Vector2 GroundSensorSize;

		public bool IsGrounded { get { return groundContactsNum > 0; } }

		public override void Start()
		{
			base.Start();

			var bodyPosition = new Vec2(BodyPosition.x, BodyPosition.y);
			var bodyHalf = new Vec2(BodySize.x, BodySize.y) * 0.5f;
			var bodyShape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					bodyPosition + new Vec2(-bodyHalf.X, -bodyHalf.Y),
					bodyPosition + new Vec2(bodyHalf.X, -bodyHalf.Y),
					bodyPosition + new Vec2(bodyHalf.X, bodyHalf.Y),
					bodyPosition + new Vec2(-bodyHalf.X, bodyHalf.Y)
				})
			};
			Body.CreateFixture(bodyShape);

			var footShape = new CircleShape(FootRadius, 1f) {
				Position = new Vec2(FootPosition.x, FootPosition.y)
			};
			Body.CreateFixture(footShape);

			var groundSensorPosition = new Vec2(GroundSensorPosition.x, GroundSensorPosition.y);
			var groundSensorHalf = new Vec2(GroundSensorSize.x, GroundSensorSize.y) * 0.5f;
			var groundSensorShape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					groundSensorPosition + new Vec2(-groundSensorHalf.X, -groundSensorHalf.Y),
					groundSensorPosition + new Vec2(groundSensorHalf.X, -groundSensorHalf.Y),
					groundSensorPosition + new Vec2(groundSensorHalf.X, groundSensorHalf.Y),
					groundSensorPosition + new Vec2(-groundSensorHalf.X, groundSensorHalf.Y)
				})
			};
			var groundSensorFixture = Body.CreateFixture(groundSensorShape);
			groundSensorFixture.IsSensor = true;
			groundSensorFixture.UserData = new PhysicsBodyData() {
				BeginContact = BeginContact,
				EndContact = EndContact
			};
		}

		private bool BeginContact(Contact contact, PhysicsContactSide side)
		{
			var fixture = side == PhysicsContactSide.A ? contact.FixtureB : contact.FixtureA;
			var userData = fixture.UserData as PhysicsBodyData;

			if (userData != null && userData.IsGround) {
				groundContactsNum++;
			}
			return true;
		}

		private void EndContact(Contact contact, PhysicsContactSide side)
		{
			var fixture = side == PhysicsContactSide.A ? contact.FixtureB : contact.FixtureA;
			var userData = fixture.UserData as PhysicsBodyData;

			if (userData != null && userData.IsGround) {
				groundContactsNum--;
			}
		}
	}
}
