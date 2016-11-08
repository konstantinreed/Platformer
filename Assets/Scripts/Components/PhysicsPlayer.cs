using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PhysicsPlayer : PhysicsBody
	{
		public Vector2 BodyPosition;
		public Vector2 BodySize;
		public Vector2 FootPosition;
		public float FootRadius;

		public override void Start()
		{
			base.Start();
			
			var bodyPosition = new Vec2(BodyPosition.x, BodyPosition.y);
			var bodyHalf = new Vec2(BodySize.x, BodySize.y) * 0.5f;
			var shapeBody = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					bodyPosition + new Vec2(-bodyHalf.X, -bodyHalf.Y),
					bodyPosition + new Vec2(bodyHalf.X, -bodyHalf.Y),
					bodyPosition + new Vec2(bodyHalf.X, bodyHalf.Y),
					bodyPosition + new Vec2(-bodyHalf.X, bodyHalf.Y)
				})
			};
			Body.CreateFixture(shapeBody);

			var shapeFoot = new CircleShape(FootRadius, 1f) {
				Position = new Vec2(FootPosition.x, FootPosition.y)
			};
			Body.CreateFixture(shapeFoot);
		}
	}
}
