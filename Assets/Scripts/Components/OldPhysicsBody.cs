using FarseerPhysics.Dynamics;
using Scripts.Physics;
using UnityEngine;

namespace Scripts
{
	public abstract class OldPhysicsBody : MonoBehaviour
	{
		private static OldPhysicsEngine Engine { get { return GameObject.FindWithTag("PhysicsEngine").GetComponent<OldPhysicsEngine>(); } }
		
		private float gravityScaleStandart;
		private float gravityScaleMultiplier;

		public bool IsDynamic;
		public bool IsFixedRotation;
		public float Mass = 20f;
		public float GravityScale = 1f;
		public float Friction = 0.2f;
		public float Restitution = 0f;

		protected OldPhysicsWorld World { get; private set; }
		public Body Body { get; protected set; }

		public float GravityScaleMultiplier
		{
			get { return gravityScaleMultiplier; }
			set
			{
				Body.GravityScale = gravityScaleStandart * gravityScaleMultiplier;
				gravityScaleMultiplier = value;
			}
		}

		public virtual void Start()
		{
			World = Engine.PhysicsWorld;

			Body = World.CreateBody(new Vector2(transform.position.x, transform.position.y));
			Body.Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			Body.BodyType = IsDynamic ? BodyType.Dynamic : BodyType.Static;
			Body.FixedRotation = IsFixedRotation;
			Body.Mass = Mass;
			Body.GravityScale = GravityScale;
			Body.Friction = Friction;
			Body.Restitution = Restitution;

			gravityScaleStandart = Body.GravityScale;
		}

		public virtual void Update()
		{
			if (Body == null) {
				Debug.LogWarningFormat("Object \"{0}\" hasn't physics body!", name);
				return;
			}

			transform.position = new Vector3(Body.Position.X, Body.Position.Y, 0f);
			if (!IsFixedRotation) {
				transform.rotation = Quaternion.AngleAxis(Body.Rotation * Mathf.Rad2Deg, Vector3.forward);
			}
		}
	}
}
