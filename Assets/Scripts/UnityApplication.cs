using UnityEngine;

namespace Scripts
{
	public class UnityApplication : MonoBehaviour
	{
		public Application Application { get; private set; }

		public void Start()
		{
			Application = new Application();
		}

		public void Update()
		{
			Application.Update(Time.deltaTime);
		}
	}
}
