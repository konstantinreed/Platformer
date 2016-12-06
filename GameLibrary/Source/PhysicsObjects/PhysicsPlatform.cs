using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using GameLibrary.Source.SerializableFormats;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class PhysicsPlatform : PhysicsBody
	{
		public readonly Fixture Fixture;

		public PhysicsPlatform(PhysicsSystem physicsSystem, PlatformFormat format)
			: base(physicsSystem, BodyType.Static, format.Position.Vector2, 0f, isFixedRotation: true)
		{
			var vertices = new Vector2[format.Vertices.Count];
			for (var i = 0; i < format.Vertices.Count; i++) {
				vertices[i] = format.Vertices[i].Vector2;
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
