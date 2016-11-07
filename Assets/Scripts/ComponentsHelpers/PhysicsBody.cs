using FarseerPhysics.Dynamics;
using Scripts.Physics;
using UnityEngine;

namespace Scripts
{
	public abstract class PhysicsBody : MonoBehaviour
	{
		private static PhysicsEngine Engine { get { return GameObject.FindWithTag("PhysicsEngine").GetComponent<PhysicsEngine>(); } }

		protected PhysicsWorld World { get; private set; }
		public Body Body { get; protected set; }

		public virtual void Start()
		{
			World = Engine.PhysicsWorld;
		}

		public virtual void Update()
		{
			if (Body == null) {
				Debug.LogWarningFormat("Object \"{0}\" hasn't physics body!", name);
				return;
			}

			transform.position = new Vector3(Body.Position.X, Body.Position.Y, 0f);
		}
	}
}
