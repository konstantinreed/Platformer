using GameLibrary.Source.SerializableFormats;
using System;

namespace GameLibrary
{
	public class GameApplication
	{
		private DateTime startDateTime;
		private int lastSimulationStep;

		public PhysicsSystem PhysicsSystem { get; private set; }
		public Level Level { get; private set; }
		public ClientManager ClientManager { get; private set; }
		public int CurrentStep { get; private set; }
		public int CurrentSimulationStep { get; private set; }

		public GameApplication(LevelFormat levelFormat)
		{
			PhysicsSystem = new PhysicsSystem();
			Level = new Level(this, levelFormat);
			ClientManager = new ClientManager(this, OnInput, Settings.PlayersCount);

			startDateTime = DateTime.Now;
		}

		private void OnInput(ClientInstance client, InputState state)
		{
			var currentStep = client.InputStates.CurrentStep;
			var rewindResult = client.InputStates.RewindToStep(state);

			if (currentStep != client.InputStates.CurrentStep) {
				foreach (var otherClient in ClientManager.Clients) {
					if (otherClient == client) {
						continue;
					}

					otherClient.InputStates.RewindForward(client.InputStates.CurrentStep);
				}
			}
			if (!rewindResult.WasRewinded) {
				return;
			}

			// TODO: Reset physics states
			//rewindResult.Steps
		}

		public void ClientUpdate()
		{
			var elapsedSeconds = (DateTime.Now - startDateTime).TotalSeconds;
			CurrentStep = (int)(elapsedSeconds / Settings.Step);
			CurrentSimulationStep = (int)(elapsedSeconds / Settings.SimulationStep);
		}

		public void ClientSimulation()
		{
			for (; lastSimulationStep <= CurrentSimulationStep; lastSimulationStep++) {
				var currentStep = (int)(lastSimulationStep * Settings.StepsPerSimulations);

				foreach (var client in ClientManager.Clients) {
					client.Input = client.InputStates[currentStep];
				}

				// TODO: Жуткая залепа для ClientSidePrediction. Надо сделать ротацию состояний для объектов
				//требующих синхронизации
				foreach (var physicsObject in PhysicsSystem.Objects) {
					var syncObject = physicsObject as PhysicsPlayer;
					if (syncObject == null) {
						continue;
					}
					syncObject.State.Step = currentStep;
				}
				PhysicsSystem.FixedUpdate();
			}
		}
	}
}
