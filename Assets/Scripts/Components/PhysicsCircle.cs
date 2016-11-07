using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using UnityEngine;

namespace Scripts
{
	public class PhysicsCircle : PhysicsBody
	{
		public float Radius = 0.5f;
		public bool IsDynamic;

		public override void Start()
		{
			base.Start();

			Body = World.CreateBody(new Vector2(transform.position.x, transform.position.y));
			Body.BodyType = IsDynamic ? BodyType.Dynamic : BodyType.Static;

			var shape = new CircleShape(Radius, 1f);
			Body.CreateFixture(shape);
		}
	}
}
