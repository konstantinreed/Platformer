using GameLibrary;
using UnityEngine;

namespace Scripts
{
	public class UnityApplication : MonoBehaviour
	{
		public GameApplication Application { get; private set; }

		public void Start()
		{
			Application = new GameApplication();
		}

		public void Update()
		{
			Application.Update(Time.deltaTime);
		}
	}
}
