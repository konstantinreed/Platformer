using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	internal class GamePhysics
	{
		private readonly Vector2 gravityForce = new Vector2(0f, -9.82f);
		private Level level;

		public World World { get; private set; }
		public readonly List<PhysicsBody> Objects = new List<PhysicsBody>();

		public GamePhysics()
		{
			World = new World(gravityForce);
		}

		public void FixedUpdate()
		{
			foreach (var physicsObject in Objects) {
				physicsObject.Update(Settings.SimulationStepF);
			}

			World.Step(Settings.SimulationStepF);

			foreach (var physicsObject in Objects) {
				physicsObject.Updated(Settings.SimulationStepF);
			}
		}

		public void LoadLevel(Level level)
		{
			this.level = level;

			foreach (var platform in level.StaticPlatforms) {
				new PhysicsPlatform(this, platform);
			}
		}

		public PhysicsPlayer SpawnPlayer()
		{
			return new PhysicsPlayer(this, level.SpawnPosition.Vector2);
		}
	}
}
