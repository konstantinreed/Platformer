using FarseerPhysics.Collision.Shapes;

namespace Scripts
{
	public class PhysicsCircle : PhysicsBody
	{
		public float Radius = 0.5f;

		public override void Start()
		{
			base.Start();

			var shape = new CircleShape(Radius, 1f);
			Body.CreateFixture(shape);
		}
	}
}
