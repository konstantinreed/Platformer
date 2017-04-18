using GameLibrary;
using UnityEngine;

namespace Scripts
{
	using Mathf = GameLibrary.Mathf;

	public class PlayerView : MonoBehaviour
	{
		private const float FlipTolerance = 0.5f;
		private const float RotationSmoothing = 0.5f;
		private const int HittingAnimationsCount = 2;

		private Player player;
		private Animator animator;
		private Quaternion initialRotation;
		private bool isFacingRight = true;
		private bool wasHitting;
		private int hitIndex;

		public GameObject AnimatorGameObject;
		public Transform RotationTransform;

		public void Start()
		{
			player = UnityApplication.Instance.Player;
			animator = AnimatorGameObject != null ? AnimatorGameObject.GetComponent<Animator>() : null;
			initialRotation = RotationTransform != null ? RotationTransform.rotation : Quaternion.identity;
		}

		public void Update()
		{
			var state = player.GetState();
			transform.position = new Vector3(state.Position.X, state.Position.Y, 0f);

			if ((state.LinearVelocity.X > FlipTolerance && !isFacingRight) || (state.LinearVelocity.X < -FlipTolerance && isFacingRight)) {
				Flip();
			}

			var angle = Mathf.Lerp(RotationSmoothing, state.ScopeAngle, 0f) * Mathf.RadToDeg;
			RotationTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * initialRotation;

			var velocityXFactor = Mathf.Abs(state.LinearVelocity.X) / PlayerState.MaxHorizontalSpeed;
			var velocityYFactor =
				state.LinearVelocity.Y /
				(state.LinearVelocity.Y >= 0 ? PlayerState.MaxVerticalSpeed : -PlayerState.MinVerticalSpeed);

			if (animator != null) {
				if (wasHitting && state.Animation != PlayerAnimation.Hitting) {
					hitIndex = (hitIndex + 1) % HittingAnimationsCount;
				}
				wasHitting = state.Animation == PlayerAnimation.Hitting;

				animator.SetFloat("VelocityX", velocityXFactor);
				animator.SetFloat("VelocityY", velocityYFactor);
				animator.SetFloat("Rotation", angle);
				animator.SetFloat("LandingVelocityYFactor", state.LandingVelocityYFactor);
				animator.SetBool("IsGroundSensorActive", state.IsGrounded);
				animator.SetBool("IsWallSensorActive", state.IsClingedWall);
				animator.SetBool("IsIdle",        state.Animation == PlayerAnimation.Idle);
				animator.SetBool("IsRunning",     state.Animation == PlayerAnimation.Running);
				animator.SetBool("IsJumping",     state.Animation == PlayerAnimation.Jumping);
				animator.SetBool("IsWallJumping", state.Animation == PlayerAnimation.WallJumping);
				animator.SetBool("IsFalling",     state.Animation == PlayerAnimation.Falling);
				animator.SetBool("IsWallFalling", state.Animation == PlayerAnimation.WallFalling);
				animator.SetBool("IsLanding",     state.Animation == PlayerAnimation.Landing);
				animator.SetBool("IsHitting",     state.Animation == PlayerAnimation.Hitting);
				animator.SetBool("IsDying",       state.Animation == PlayerAnimation.Dying);
				animator.SetInteger("HittingIndex", hitIndex);
			}
		}

		private void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
