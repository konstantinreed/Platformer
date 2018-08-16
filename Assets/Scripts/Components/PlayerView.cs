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
		private bool wasHitting;
		private int hitIndex;

		public GameObject AnimatorGameObject;
		public Transform RotationTransform;
		public PlayerState State { get; private set; }
		public bool IsFacingRight { get; private set; }

		public void Start()
		{
			player = UnityApplication.Instance.Player;
			State = player.GetState();
			animator = AnimatorGameObject != null ? AnimatorGameObject.GetComponent<Animator>() : null;
			initialRotation = RotationTransform != null ? RotationTransform.rotation : Quaternion.identity;
			IsFacingRight = true;
        }

		public void Update()
		{
			State = player.GetState();
			transform.position = new Vector3(State.Position.X, State.Position.Y, 0f);

			if ((State.LinearVelocity.X > FlipTolerance && !IsFacingRight) || (State.LinearVelocity.X < -FlipTolerance && IsFacingRight)) {
				Flip();
			}

			var angle = Mathf.Lerp(RotationSmoothing, State.ScopeAngle, 0f) * Mathf.RadToDeg;
			RotationTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * initialRotation;

			var velocityXFactor = Mathf.Abs(State.LinearVelocity.X) / PlayerState.MaxHorizontalSpeed;
			var velocityYFactor =
				State.LinearVelocity.Y /
				(State.LinearVelocity.Y >= 0 ? PlayerState.MaxVerticalSpeed : -PlayerState.MinVerticalSpeed);

			if (animator != null) {
				if (wasHitting && State.Animation != PlayerAnimation.Hitting) {
					hitIndex = (hitIndex + 1) % HittingAnimationsCount;
				}
				wasHitting = State.Animation == PlayerAnimation.Hitting;

				animator.SetFloat("VelocityX", velocityXFactor);
				animator.SetFloat("VelocityY", velocityYFactor);
				animator.SetFloat("Rotation", angle);
				animator.SetFloat("LandingVelocityYFactor", State.LandingVelocityYFactor);
				animator.SetBool("IsGroundSensorActive", State.IsGrounded);
				animator.SetBool("IsWallSensorActive", State.IsClingedWall);
				animator.SetBool("IsIdle",        State.Animation == PlayerAnimation.Idle);
				animator.SetBool("IsRunning",     State.Animation == PlayerAnimation.Running);
				animator.SetBool("IsJumping",     State.Animation == PlayerAnimation.Jumping);
				animator.SetBool("IsWallJumping", State.Animation == PlayerAnimation.WallJumping);
				animator.SetBool("IsFalling",     State.Animation == PlayerAnimation.Falling);
				animator.SetBool("IsWallFalling", State.Animation == PlayerAnimation.WallFalling);
				animator.SetBool("IsLanding",     State.Animation == PlayerAnimation.Landing);
				animator.SetBool("IsHitting",     State.Animation == PlayerAnimation.Hitting);
				animator.SetBool("IsDying",       State.Animation == PlayerAnimation.Dying);
				animator.SetInteger("HittingIndex", hitIndex);
			}
		}

		private void Flip()
		{
			IsFacingRight = !IsFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
