namespace GameLibrary
{
	public class GameApplication
	{
		public PhysicsSystem PhysicsSystem { get; private set; }
		public Level Level { get; private set; }

		public GameApplication()
		{
			PhysicsSystem = new PhysicsSystem();

			Level = new Level(this);
			Level.Load();
		}

		public void Update(float delta)
		{
			PhysicsSystem.Update(delta);
		}
	}
}
