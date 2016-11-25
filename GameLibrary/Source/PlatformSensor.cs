using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class PlatformSensor
	{
		private readonly World world;
		private readonly Vector2 a;
		private readonly Vector2 b;
		private PolygonShape sensorShape;
		private Transform sensorTransform;
		private Rot sensorRotation = new Rot(0f);

		public bool IsActive { get; private set; }

		public PlatformSensor(World world, float width, float height, float offsetX, float offsetY)
		{
			this.world = world;

			var offset = new Vector2(offsetX, offsetY);
			var halfSize = new Vector2(width, height) * 0.5f;
			a = offset - halfSize;
			b = offset + halfSize;

			var vertices = new Vertices(new[] {
				offset + new Vector2(-halfSize.X, -halfSize.Y),
				offset + new Vector2(halfSize.X, -halfSize.Y),
				offset + new Vector2(halfSize.X, halfSize.Y),
				offset + new Vector2(-halfSize.X, halfSize.Y)
			});
			sensorShape = new PolygonShape(vertices, 1f);
		}

		public void Update(Vector2 position)
		{
			IsActive = false;

			var aabb = new AABB(a + position, b + position);
			sensorTransform = new Transform(ref position, ref sensorRotation);
			world.QueryAABB(PlatformSearching, ref aabb);
		}

		private bool PlatformSearching(Fixture fixture)
		{
			var bodyData = fixture.UserData as PhysicsBodyData;
			if (bodyData != null && bodyData.IsPlatform) {
				Transform transform;
				fixture.Body.GetTransform(out transform);
				if (Collision.TestOverlap(fixture.Shape, 0, sensorShape, 0, ref transform, ref sensorTransform)) {
					IsActive = true;
					return false;
				}
			}
			return true; // Continue searching platform
		}
	}
}
