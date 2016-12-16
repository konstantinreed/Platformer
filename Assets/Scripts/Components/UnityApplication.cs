using GameLibrary;
using GameLibrary.Source.SerializableFormats;
using System.Collections.Generic;
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
			var levelFormat = GetSampleLevel();

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

		private LevelFormat GetSampleLevel()
		{
			return new LevelFormat() {
				SpawnPosition = new VectorFormat(3f, 4.5f),
				StaticPlatforms = new List<PlatformFormat>() {
					new PlatformFormat() {
						Position = new VectorFormat(-5.5f, 1f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-5f, -0.5f),
							new VectorFormat(5f, -0.5f),
							new VectorFormat(5f, 0.5f),
							new VectorFormat(-5f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(26f, -1f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-25f, -0.5f),
							new VectorFormat(25f, -0.5f),
							new VectorFormat(25f, 0.5f),
							new VectorFormat(-25f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(12f, 4f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-10f, -0.5f),
							new VectorFormat(10f, -0.5f),
							new VectorFormat(10f, 0.5f),
							new VectorFormat(-10f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(39f, 4f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-10f, -0.5f),
							new VectorFormat(10f, -0.5f),
							new VectorFormat(10f, 0.5f),
							new VectorFormat(-10f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(-10f, 9.25f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-0.5f, -7.75f),
							new VectorFormat(0.5f, -7.75f),
							new VectorFormat(0.5f, 7.75f),
							new VectorFormat(-0.5f, 7.75f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(50.5f, 4.5f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-0.5f, -5f),
							new VectorFormat(0.5f, -5f),
							new VectorFormat(0.5f, 5f),
							new VectorFormat(-0.5f, 5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(-6f, 6.5f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-0.5f, -1f),
							new VectorFormat(0.5f, -1f),
							new VectorFormat(0.5f, 1f),
							new VectorFormat(-0.5f, 1f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(1.5f, 16.5f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-11f, -0.5f),
							new VectorFormat(11f, -0.5f),
							new VectorFormat(11f, 0.5f),
							new VectorFormat(-11f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(10.5f, 11f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-2f, -0.5f),
							new VectorFormat(2f, -0.5f),
							new VectorFormat(2f, 0.5f),
							new VectorFormat(-2f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(13f, 11.75f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-0.5f, -5.25f),
							new VectorFormat(0.5f, -5.25f),
							new VectorFormat(0.5f, 5.25f),
							new VectorFormat(-0.5f, 5.25f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(2f, 11f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-3.5f, -0.5f),
							new VectorFormat(3.5f, -0.5f),
							new VectorFormat(3.5f, 0.5f),
							new VectorFormat(-3.5f, 0.5f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(0.383884f, -0.090991f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(0.883883f, -1.59099f),
							new VectorFormat(1.59099f, -0.883883f),
							new VectorFormat(-0.883883f, 1.59099f),
							new VectorFormat(-1.59099f, 0.883883f),
						}
					},
					new PlatformFormat() {
						Position = new VectorFormat(-3.61f, 8.67f),
						Vertices = new List<VectorFormat>() {
							new VectorFormat(-2.828427f, -2.12132f),
							new VectorFormat(-2.12132f, -2.828427f),
							new VectorFormat(2.828427f, 2.12132f),
							new VectorFormat(2.12132f, 2.828427f),
						}
					},
				}
			};
		}
	}
}
