using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
	internal class GamePhysics
	{
		private readonly Vector2 gravityForce = new Vector2(0f, -9.82f);

		public Level Level;
		public World World { get; private set; }
		public readonly List<GameLogic> Logics = new List<GameLogic>() {
			new KeepPlayersAlive()
		};
		public readonly List<PhysicsBody> Objects = new List<PhysicsBody>();

		public GamePhysics()
		{
			World = new World(gravityForce);
		}

		public void LoadLevel(Level level)
		{
			Level = level;

			foreach (var platform in level.StaticPlatforms) {
				new PhysicsPlatform(this, platform);
			}
		}

		public PhysicsPlayer SpawnPlayer()
		{
			return new PhysicsPlayer(this, Level.SpawnPosition.Vector2);
		}

		public void Awake()
		{
			foreach (var logic in Logics) {
				logic.Awake();
			}
		}

		public void FixedUpdate()
		{
			foreach (var logic in Logics) {
				logic.Update();
			}
			foreach (var physicsObject in Objects) {
				physicsObject.Update(Settings.SimulationStepF);
			}

			World.Step(Settings.SimulationStepF);

			foreach (var physicsObject in Objects) {
				physicsObject.Updated(Settings.SimulationStepF);
			}
			foreach (var logic in Logics) {
				logic.Updated();
			}
		}
	}
}
