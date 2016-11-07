using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PhysicsBox : PhysicsBody
	{
		public float HalfWidth = 0.5f;
		public float HalfHeight = 0.5f;
		public bool IsDynamic;

		public override void Start()
		{
			base.Start();

			Body = World.CreateBody(new Vector2(transform.position.x, transform.position.y));
			Body.BodyType = IsDynamic ? BodyType.Dynamic : BodyType.Static;

			var shape = new PolygonShape(1f) {
				Vertices = new Vertices(new[] {
					new Vec2(-HalfWidth, -HalfHeight),
					new Vec2(HalfWidth, -HalfHeight),
					new Vec2(HalfWidth, HalfHeight),
					new Vec2(-HalfWidth, HalfHeight),
				})
			};
			Body.CreateFixture(shape);
		}
	}
}
