using GameLibrary;
using UnityEngine;

namespace Scripts
{
	public class UnityApplication : MonoBehaviour
	{
		public static UnityApplication Instance {
			get { return GameObject.FindGameObjectWithTag("Application").GetComponent<UnityApplication>(); }
		}

		private bool WasJumpPressedDuringStep;
		private int JumpPressedStep;

		public GameApplication Application { get; private set; }
		public ClientInstance Client { get; private set; }
		public int CurrentStep { get; private set; }
		public int CurrentSimulationStep { get; private set; }

		// Server application // TODO: Rework
		private GameApplication serverApplication;
		private ClientInstance serverClient;

		public void Start()
		{
			// Load sample level
			var levelComponent = GetComponent<SampleLevel>();
			var levelFormat = levelComponent.GetLevel();

			Application = new GameApplication(levelFormat);
			Client = Application.ClientManager.Clients[0];

			// Server application // TODO: Rework
			serverApplication = new GameApplication(levelFormat);
			serverClient = serverApplication.ClientManager.Clients[0];

			SendInput(0);
			CurrentStep = Application.CurrentStep;
		}

		public void Update()
		{
			Application.ClientUpdate();

			WasJumpPressedDuringStep = WasJumpPressedDuringStep || Input.GetKeyDown(KeyCode.Space);
			if (CurrentStep < Application.CurrentStep) {
				CurrentStep = Application.CurrentStep;
				if (WasJumpPressedDuringStep) {
					JumpPressedStep = CurrentStep;
				}
				SendInput(CurrentStep);
				WasJumpPressedDuringStep = false;
			}

			if (CurrentSimulationStep < Application.CurrentSimulationStep) {
				CurrentSimulationStep = Application.CurrentSimulationStep;
				Application.ClientSimulation();
			}
		}

		private void SendInput(int step)
		{
			var inputState = new InputState() {
				Step = step,
				IsLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow),
				IsRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow),
				IsJumpPressed = Input.GetKey(KeyCode.Space),
				JumpPressedStep = JumpPressedStep
			};
			Client.SendInput(inputState);

			// Server application // TODO: Rework
			serverClient.SendInput(inputState);
		}
	}
}
