using GameLibrary.Source.SerializableFormats;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	public class Level
	{
		private readonly GameApplication application;
		private readonly Vector2 spawnPosition;

		public Level(GameApplication application, LevelFormat format)
		{
			this.application = application;

			spawnPosition = format.SpawnPosition.Vector2;
			foreach (var platform in format.StaticPlatforms) {
				new PhysicsPlatform(application.PhysicsSystem, platform);
			}
		}

		public PhysicsPlayer SpawnPlayer()
		{
			return new PhysicsPlayer(application.PhysicsSystem, spawnPosition);
		}
	}
}
