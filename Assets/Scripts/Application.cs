namespace Scripts
{
	public class Application
	{
		public PhysicsSystem PhysicsSystem { get; private set; }

		public Application()
		{
			PhysicsSystem = new PhysicsSystem();
		}

		public void Update(float delta)
		{
			PhysicsSystem.Update(delta);
		}
	}
}
