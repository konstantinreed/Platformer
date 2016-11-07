using Scripts.Physics;
using UnityEngine;

namespace Scripts
{
	public class PhysicsEngine : MonoBehaviour
	{
		public PhysicsWorld PhysicsWorld { get; private set; }

		public void Start ()
		{
			PhysicsWorld = new PhysicsWorld();
		}

		public void Update ()
		{
			PhysicsWorld.Update(Time.deltaTime);
		}
	}
}
