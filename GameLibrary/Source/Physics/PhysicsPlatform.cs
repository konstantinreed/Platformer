using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	internal class PhysicsPlatform : PhysicsBody
	{
		public readonly Fixture Fixture;

		public PhysicsPlatform(GamePhysics physicsSystem, Platform platform)
			: base(physicsSystem, BodyType.Static, platform.Position.Vector2, 0f, isFixedRotation: true)
		{
			var vertices = new Vector2[platform.Vertices.Count];
			for (var i = 0; i < platform.Vertices.Count; i++) {
				vertices[i] = platform.Vertices[i].Vector2;
			}

			var bodyShape = new PolygonShape(1f) {
				Vertices = new Vertices(vertices)
			};
			Fixture = Body.CreateFixture(bodyShape);
			Fixture.UserData = new PhysicsBodyData() {
				IsPlatform = true
			};
		}
	}
}
