using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PlayerController : MonoBehaviour
	{
		private const float MaxHorizontalSpeed = 10f;
		private const float MaxVerticalSpeed = 10f;

		private PhysicsPlayer physicsPlayer;
		private Animator animator;
		private bool isFacingRight = true;

		public GameObject AnimatorGameObject;

		public void Start()
		{
			physicsPlayer = GetComponent<PhysicsPlayer>();
			animator = AnimatorGameObject != null ? AnimatorGameObject.GetComponent<Animator>() : null;
		}

		public void Update()
		{
			if (physicsPlayer == null || physicsPlayer.Body == null) {
				Debug.LogWarningFormat("Object \"{0}\" hasn't physics body!", name);
				return;
			}

			var velocityX = Input.GetAxis("Horizontal") * MaxHorizontalSpeed;
			var velocityY = physicsPlayer.Body.LinearVelocity.Y;
			if (physicsPlayer.IsGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))) {
				velocityY = MaxVerticalSpeed;
			}

			physicsPlayer.Body.LinearVelocity = new Vec2(velocityX, velocityY);
			if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight)) {
				Flip();
			}

			if (animator != null) {
				animator.SetFloat("VelocityX", Mathf.Abs(velocityX));
				animator.SetFloat("VelocityY", Mathf.Abs(velocityY));
				animator.SetBool("IsGrounded", physicsPlayer.IsGrounded);
			}
		}

		public void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
