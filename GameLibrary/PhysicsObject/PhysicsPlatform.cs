using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class PhysicsPlatform : PhysicsBody
	{
		public readonly Fixture Fixture;

		public PhysicsPlatform(GameApplication application, Vector2 position, float rotation, float halfWidth, float halfHeight)
			: base(application, BodyType.Static, position, rotation, isFixedRotation: true)
		{
			var bodyShape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					new Vector2(-halfWidth, -halfHeight),
					new Vector2(halfWidth, -halfHeight),
					new Vector2(halfWidth, halfHeight),
					new Vector2(-halfWidth, halfHeight),
				})
			};
			Fixture = Body.CreateFixture(bodyShape);
			Fixture.UserData = new PhysicsBodyData() {
				IsPlatform = true
			};
		}
	}
}
