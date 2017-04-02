using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	internal class PlatformSensor
	{
		internal struct ScopeSensorData
		{
			public Vector2 From;
			public Vector2 To;
		}

		internal class ScopeSensor
		{
			public bool IsActive;
			public float Radians;
			public float Fraction;
		}

		private readonly World world;
		private readonly Vector2 a;
		private readonly Vector2 b;
		private readonly PolygonShape sensorShape;
		private readonly ScopeSensorData[] scopeSensorsData;
		private Transform sensorTransform;
		private Rot sensorRotation = new Rot(0f);
		private int currentScopeIndex;

		public bool IsActive { get; private set; }
		public ScopeSensor[] ScopeSensors { get; private set; }

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
				ScopeSensors = new ScopeSensor[scopeSensorsData.Length];
				for (int i = 0; i < scopeSensorsData.Length; i++) {
					ScopeSensors[i] = new ScopeSensor();
				}
			}
		}

		public void Update(Vector2 position)
		{
			IsActive = false;
			if (scopeSensorsData != null) {
				for (int i = 0; i < scopeSensorsData.Length; i++) {
					ScopeSensors[i].IsActive = false;
				}
			}

			var aabb = new AABB(a + position, b + position);
			sensorTransform = new Transform(ref position, ref sensorRotation);
			world.QueryAABB(TestPlatformCollision, ref aabb);

			if (!IsActive || scopeSensorsData == null) {
				return;
			}
			for (currentScopeIndex = 0; currentScopeIndex < scopeSensorsData.Length; currentScopeIndex++) {
				world.RayCast(
					TestPlatformCollision,
					position + scopeSensorsData[currentScopeIndex].From,
					position + scopeSensorsData[currentScopeIndex].To
				);
			}
		}

		public void Deactivate()
		{
			IsActive = false;
			if (scopeSensorsData != null) {
				for (int i = 0; i < scopeSensorsData.Length; i++) {
					ScopeSensors[i].IsActive = false;
				}
			}
		}

		public float GetAverageRadians(float defaultRadians = 0f)
		{
			if (scopeSensorsData == null) {
				return defaultRadians;
			}

			var scopesCount = 0f;
			var radiansSumm = 0f;
			for (var i = 0; i < ScopeSensors.Length; i++) {
				var scopeSensor = ScopeSensors[i];
				if (!scopeSensor.IsActive) {
					continue;
				}

				radiansSumm += scopeSensor.Radians;
				++scopesCount;
			}
			return scopesCount > 0 ? radiansSumm / scopesCount : defaultRadians;
		}

		public float GetMinimalFraction(float defaultFraction = 1f)
		{
			if (scopeSensorsData == null) {
				return defaultFraction;
			}

			var scopesCount = 0f;
			var minimalFraction = float.MaxValue;
			for (var i = 0; i < ScopeSensors.Length; i++) {
				var scopeSensor = ScopeSensors[i];
				if (!scopeSensor.IsActive || minimalFraction < scopeSensor.Fraction) {
					continue;
				}

				minimalFraction = scopeSensor.Fraction;
				++scopesCount;
			}
			return scopesCount > 0 ? minimalFraction : defaultFraction;
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

			var scopeSensor = ScopeSensors[currentScopeIndex];
			if (!scopeSensor.IsActive || fraction < scopeSensor.Fraction) {
				scopeSensor.IsActive = true;
				scopeSensor.Fraction = fraction;
				scopeSensor.Radians = Mathf.Atan2(normal);
			}
			return 1f; // Continue platform searching
		}
	}
}
