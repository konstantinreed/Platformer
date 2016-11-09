using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Scripts.Physics;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PhysicsBox : PhysicsBody
	{
		public bool IsGround;
		public float HalfWidth = 0.5f;
		public float HalfHeight = 0.5f;

		public override void Start()
		{
			base.Start();

			var bodyShape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					new Vec2(-HalfWidth, -HalfHeight),
					new Vec2(HalfWidth, -HalfHeight),
					new Vec2(HalfWidth, HalfHeight),
					new Vec2(-HalfWidth, HalfHeight),
				})
			};
			var bodyFixture = Body.CreateFixture(bodyShape);
			bodyFixture.UserData = new PhysicsBodyData() {
				IsGround = IsGround
			};
		}
	}
}
