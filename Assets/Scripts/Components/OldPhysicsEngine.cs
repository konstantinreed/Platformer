using Scripts.Physics;
using UnityEngine;

namespace Scripts
{
	public class OldPhysicsEngine : MonoBehaviour
	{
		public OldPhysicsWorld PhysicsWorld { get; private set; }

		public void Start ()
		{
			PhysicsWorld = new OldPhysicsWorld();
		}

		public void Update ()
		{
			PhysicsWorld.Update(Time.deltaTime);
		}
	}
}
