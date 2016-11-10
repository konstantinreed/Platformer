using System;
using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PlayerController : MonoBehaviour
	{
		private static readonly TimeSpan noGravityScaleTimeAfterJump = TimeSpan.FromSeconds(0.1d);

		private PhysicsPlayer physicsPlayer;
		private Animator animator;
		private bool isFacingRight = true;
		private DateTime jumpedTime = DateTime.Now - noGravityScaleTimeAfterJump;

		public float MaxHorizontalSpeed = 10f;
		public float MaxVerticalSpeed = 10f;
		public float GravityScaleMultiplierGrounded = 1f;
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

			var isJumping = physicsPlayer.IsGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W));
			var velocityX = Input.GetAxis("Horizontal") * MaxHorizontalSpeed;
			var velocityY = physicsPlayer.Body.LinearVelocity.Y;
			if (isJumping) {
				velocityY = MaxVerticalSpeed;
				jumpedTime = DateTime.Now;
			}

			physicsPlayer.Body.LinearVelocity = new Vec2(velocityX, velocityY);
			if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight)) {
				Flip();
			}

			var requiredGravityMultiplier =
				!isJumping &&
				physicsPlayer.IsGrounded &&
				(DateTime.Now - jumpedTime >= noGravityScaleTimeAfterJump);
			physicsPlayer.GravityScaleMultiplier = requiredGravityMultiplier ? GravityScaleMultiplierGrounded : 1f;

			if (animator != null) {
				animator.SetFloat("VelocityX", Mathf.Abs(velocityX));
				animator.SetFloat("VelocityY", velocityY);
				animator.SetBool("IsGrounded", requiredGravityMultiplier);
			}
		}

		public void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
