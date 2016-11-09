using FarseerPhysics.Dynamics;
using Scripts.Physics;
using UnityEngine;

namespace Scripts
{
	public abstract class PhysicsBody : MonoBehaviour
	{
		private static PhysicsEngine Engine { get { return GameObject.FindWithTag("PhysicsEngine").GetComponent<PhysicsEngine>(); } }

		public bool IsDynamic;
		public bool IsFixedRotation;

		protected PhysicsWorld World { get; private set; }
		public Body Body { get; protected set; }

		public virtual void Start()
		{
			World = Engine.PhysicsWorld;

			Body = World.CreateBody(new Vector2(transform.position.x, transform.position.y));
			Body.Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			Body.BodyType = IsDynamic ? BodyType.Dynamic : BodyType.Static;
			Body.FixedRotation = IsFixedRotation;
		}

		public virtual void Update()
		{
			if (Body == null) {
				Debug.LogWarningFormat("Object \"{0}\" hasn't physics body!", name);
				return;
			}

			transform.position = new Vector3(Body.Position.X, Body.Position.Y, 0f);
			transform.rotation = Quaternion.AngleAxis(Body.Rotation * Mathf.Rad2Deg, Vector3.forward);
		}
	}
}
