using GameLibrary;
using UnityEngine;

namespace Scripts
{
	public class UnityApplication : MonoBehaviour
	{
		public static UnityApplication Instance
		{
			get { return GameObject.FindGameObjectWithTag("Application").GetComponent<UnityApplication>(); }
		}

		public GameApplication Application { get; private set; }
		public Player Player { get; private set; }

		public void Start()
		{
			var levelComponent = GetComponent<SampleLevel>();
			var level = levelComponent.GetLevel();

			Application = new GameApplication(level);
			Player = Application.SpawnPlayer();
		}

		public void Update()
		{
			Application.Update();

			var inputState = new InputState() {
				IsLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow),
				IsRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow),
				IsJumpPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space),
				IsHitPressed = Input.GetKey(KeyCode.Return),
				IsSuicidePressed = Input.GetKey(KeyCode.K)
			};
			Player.SetInput(inputState);

			Application.PhysicsSimulate();
		}
	}
}
