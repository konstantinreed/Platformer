using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PlayerController : MonoBehaviour
	{
		private const float MaxHorizontalSpeed = 10f;
		private const float MaxVerticalSpeed = 10f;

		private PhysicsBody physicsBody;
		private Animator animator;
		private bool isFacingRight = true;

		public void Start()
		{
			physicsBody = GetComponent<PhysicsBody>();
			animator = GetComponent<Animator>();
		}

		public void Update()
		{
			if (physicsBody == null || physicsBody.Body == null) {
				Debug.LogWarningFormat("Object \"{0}\" hasn't physics body!", name);
				return;
			}

			var velocityX = Input.GetAxis("Horizontal") * MaxHorizontalSpeed;
			var velocityY = Input.GetAxis("Vertical") * MaxVerticalSpeed;
			if (velocityY <= Mathf.Epsilon) {
				velocityY = physicsBody.Body.LinearVelocity.Y;
			}

			physicsBody.Body.LinearVelocity = new Vec2(velocityX, velocityY);
			if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight)) {
				Flip();
			}

			if (animator != null) {
				animator.SetFloat("VelocityX", velocityX);
				animator.SetFloat("VelocityY", velocityY);
			}
		}

		public void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
