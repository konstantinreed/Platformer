using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class PlatformSensor
	{
		private readonly World world;
		private readonly Vector2 a;
		private readonly Vector2 b;

		public bool IsActive { get; private set; }

		public PlatformSensor(World world, float width, float height, float offsetX, float offsetY)
		{
			this.world = world;

			var offset = new Vector2(offsetX, offsetY);
			var halfSize = new Vector2(width, height) * 0.5f;
			a = offset - halfSize;
			b = offset + halfSize;
		}

		public void Update(Vector2 position)
		{
			IsActive = false;

			var aabb = new AABB(a + position, b + position);
			world.QueryAABB(PlatformSearching, ref aabb);
		}

		private bool PlatformSearching(Fixture fixture)
		{
			var bodyData = fixture.UserData as PhysicsBodyData;
			if (bodyData != null && bodyData.IsPlatform) {
				IsActive = true;
				return false;
			}
			return true; // Continue searching platform
		}
	}
}
