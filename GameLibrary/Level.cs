using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class Level
	{
		private readonly GameApplication application;

		public Level(GameApplication application)
		{
			this.application = application;
		}

		public void Load()
		{
			new PhysicsPlatform(application, new Vector2(-5.5f, 1f), 0f, 5f, 0.5f);
			new PhysicsPlatform(application, new Vector2(0.39f, -0.09f), 45f * Mathf.DegToRad, 0.5f, 1.75f);
			new PhysicsPlatform(application, new Vector2(26f, -1f), 0f, 25f, 0.5f);
			new PhysicsPlatform(application, new Vector2(12f, 4f), 0f, 10f, 0.5f);
			new PhysicsPlatform(application, new Vector2(39f, 4f), 0f, 10f, 0.5f);

			new PhysicsPlatform(application, new Vector2(-10f, 5f), 0f, 0.5f, 3.5f);
			new PhysicsPlatform(application, new Vector2(50.5f, 4.5f), 0f, 0.5f, 5f);
		}
	}
}
