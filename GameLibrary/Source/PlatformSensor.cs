using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class PlatformSensor
	{
		public struct ScopeSensorData
		{
			public Vector2 From;
			public Vector2 To;
		}

		private readonly World world;
		private readonly Vector2 a;
		private readonly Vector2 b;
		private readonly PolygonShape sensorShape;
		private readonly ScopeSensorData[] scopeSensorsData;
		private readonly float[] scopeRadians;
		private Transform sensorTransform;
		private Rot sensorRotation = new Rot(0f);
		private int currentScopeIndex;
		private float currentScopeFraction;

		public bool IsActive { get; private set; }
		public float Radians { get; private set; }

		public PlatformSensor(World world, Vector2 size, Vector2 offset, ScopeSensorData[] scopeSensorsData = null)
		{
			this.world = world;
			
			var halfSize = size * 0.5f;
			a = offset - halfSize;
			b = offset + halfSize;

			var vertices = new Vertices(new[] {
				offset + new Vector2(-halfSize.X, -halfSize.Y),
				offset + new Vector2(halfSize.X, -halfSize.Y),
				offset + new Vector2(halfSize.X, halfSize.Y),
				offset + new Vector2(-halfSize.X, halfSize.Y)
			});
			sensorShape = new PolygonShape(vertices, 1f);

			if (scopeSensorsData != null && scopeSensorsData.Length > 0) {
				this.scopeSensorsData = scopeSensorsData;
				scopeRadians = new float[scopeSensorsData.Length];
			}
		}

		public void Update(Vector2 position)
		{
			IsActive = false;
			Radians = 0f;

			var aabb = new AABB(a + position, b + position);
			sensorTransform = new Transform(ref position, ref sensorRotation);
			world.QueryAABB(TestPlatformCollision, ref aabb);

			if (!IsActive || scopeSensorsData == null) {
				return;
			}
			currentScopeIndex = 0;
			for (var i = 0; i < scopeSensorsData.Length; i++) {
				currentScopeFraction = float.MaxValue;
				world.RayCast(TestPlatformCollision, position + scopeSensorsData[i].From, position + scopeSensorsData[i].To);
				if (currentScopeFraction < float.MaxValue) {
					++currentScopeIndex;
				}
			}

			if (currentScopeIndex == 0) {
				return;
			}
			var radiansSumm = 0f;
			for (var i = 0; i < currentScopeIndex; i++) {
				radiansSumm += scopeRadians[i];
			}
			Radians = radiansSumm / currentScopeIndex - Mathf.HalfPi;
		}

		public void Deactivate()
		{
			IsActive = false;
			Radians = 0f;
		}
		
		private bool TestPlatformCollision(Fixture fixture)
		{
			var bodyData = fixture.UserData as PhysicsBodyData;
			if (bodyData == null || !bodyData.IsPlatform) {
				return true; // Continue platform searching
			}

			Transform transform;
			fixture.Body.GetTransform(out transform);
			if (!Collision.TestOverlap(fixture.Shape, 0, sensorShape, 0, ref transform, ref sensorTransform)) {
				return true; // Continue platform searching
			}

			IsActive = true;
			return false; // Stop platform searching
		}

		private float TestPlatformCollision(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
		{
			var bodyData = fixture.UserData as PhysicsBodyData;
			if (bodyData == null || !bodyData.IsPlatform) {
				return -1f; // Ignore fixture and continue platform searching
			}

			if (fraction < currentScopeFraction) {
				currentScopeFraction = fraction;
				scopeRadians[currentScopeIndex] = Mathf.Atan2(normal);
			}
			return 1f; // Continue platform searching
		}
	}
}
