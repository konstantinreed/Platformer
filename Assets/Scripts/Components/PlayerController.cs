using System;
using UnityEngine;

namespace Scripts
{
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PlayerController : MonoBehaviour
	{
		private enum State
		{
			Idle,
			Running,
			Jumping,
			Falling,
			Landing
		}

		private static readonly TimeSpan noGravityScaleTimeAfterJump = TimeSpan.FromSeconds(0.1d);

		private PhysicsPlayer physicsPlayer;
		private Animator animator;
		private State state = State.Idle;
		private bool isFacingRight = true;
		private DateTime jumpedTime = DateTime.Now - noGravityScaleTimeAfterJump;

		public float MaxHorizontalSpeed = 10f;
		public float MinVerticalSpeed = -30f;
		public float MaxVerticalSpeed = 10f;
		public float GravityScaleMultiplierGrounded = 1f;
		public GameObject AnimatorGameObject;

		public bool IsKeyJumpDown { get { return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W); } }

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

			var velocityX = physicsPlayer.Body.LinearVelocity.X;
			var velocityY = Mathf.Clamp(physicsPlayer.Body.LinearVelocity.Y, MinVerticalSpeed, MaxVerticalSpeed);

			// Vertical velocity
			if (CanJump() && IsKeyJumpDown) {
				state = State.Jumping;
				velocityY = MaxVerticalSpeed;
				jumpedTime = DateTime.Now;
			}
			if (!physicsPlayer.IsGrounded && velocityY <= 0) {
				state = State.Falling;
			}
			if (
				physicsPlayer.IsGrounded &&
				(
					(state == State.Jumping && DateTime.Now - jumpedTime >= noGravityScaleTimeAfterJump) ||
					state == State.Falling
				)
			) {
				state = State.Landing;
			}
			var isOnGround = state == State.Idle || state == State.Running || state == State.Landing;

			// Horizontal velocity
			velocityX = Input.GetAxis("Horizontal") * MaxHorizontalSpeed;

			// Apply physics
			physicsPlayer.Body.LinearVelocity = new Vec2(velocityX, velocityY);
			if ((velocityX > 0 && !isFacingRight) || (velocityX < 0 && isFacingRight)) {
				Flip();
			}
			physicsPlayer.GravityScaleMultiplier = isOnGround ? GravityScaleMultiplierGrounded : 1f;

			if (animator != null) {
				animator.SetFloat("VelocityX", Mathf.Abs(velocityX) / MaxHorizontalSpeed);
				animator.SetFloat("VelocityY", velocityY / (velocityY >= 0 ? MaxVerticalSpeed : MinVerticalSpeed));
				animator.SetBool("IsGroundSensorActive", isOnGround);
				animator.SetBool("IsJumping", state == State.Jumping);
				animator.SetBool("IsFalling", state == State.Falling);
				animator.SetBool("IsLanding", state == State.Landing);
			}
		}

		public void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}

		private bool CanJump()
		{
			return
				state != State.Jumping &&
				state != State.Falling &&
				physicsPlayer.IsGrounded &&
				DateTime.Now - jumpedTime >= noGravityScaleTimeAfterJump;
		}
	}
}
