using System;
using System.Collections.Generic;

namespace GameLibrary
{
	public class GameApplication
	{
		private DateTime? startDateTime;
		private int lastSimulationStep;

		internal GamePhysics Physics { get; } = new GamePhysics();
		internal List<Player> Players { get; } = new List<Player>();
		internal float CurrentStepProgress { get; private set; }

		public int CurrentStep { get; private set; }

		public GameApplication(Level level)
		{
			The.Application = this;
            Physics.LoadLevel(level);
		}

		public Player SpawnPlayer()
		{
			var player = new Player(Physics.SpawnPlayer());
			Players.Add(player);
			return player;
		}

		public void Update()
		{
			if (!startDateTime.HasValue) {
				startDateTime = DateTime.Now;
				Physics.Awake();
			}

			var elapsedSeconds = (DateTime.Now - startDateTime.Value).TotalSeconds;
			var currentStep = elapsedSeconds / Settings.SimulationStep;
            CurrentStep = (int)currentStep;
			CurrentStepProgress = (float)(elapsedSeconds % Settings.SimulationStep) / Settings.SimulationStepF;
        }

		public void PhysicsSimulate()
		{
			if (lastSimulationStep == CurrentStep + 1) {
				var wasPreSimulationModified = false;
				foreach (var player in Players) {
					if (player.InputModified) {
						wasPreSimulationModified = true;
						break;
					}
				}
				if (wasPreSimulationModified) {
					foreach (var physicsObject in Physics.Objects) {
						var dynamicBody = physicsObject as PhysicsDynamicBody;
						dynamicBody?.RewindBack();
					}
					Physics.FixedUpdate();
				}
			}

			while (lastSimulationStep <= CurrentStep) {
				foreach (var player in Players) {
					player.RewindForward();
				}
				foreach (var physicsObject in Physics.Objects) {
					var dynamicBody = physicsObject as PhysicsDynamicBody;
					dynamicBody?.RewindForward();
				}

				Physics.FixedUpdate();
				++lastSimulationStep;
            }
		}
	}
}
