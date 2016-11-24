using GameLibrary;
using UnityEngine;

namespace Scripts
{
	using Mathf = GameLibrary.Mathf;

	public class PlayerView : MonoBehaviour
	{
		private const float FlipTolerance = 0.05f;

		private PhysicsPlayer physicsPlayer;
		private Animator animator;
		private bool isFacingRight = true;

		public GameObject AnimatorGameObject;

		public void Start()
		{
			var unityApplication = UnityApplication.Instance;
			physicsPlayer = unityApplication.Client.Player;
			animator = AnimatorGameObject != null ? AnimatorGameObject.GetComponent<Animator>() : null;
		}

		public void Update()
		{
			transform.position = new Vector3(physicsPlayer.Body.Position.X, physicsPlayer.Body.Position.Y, 0f);

			var velocity = physicsPlayer.Body.LinearVelocity;

			if ((velocity.X > FlipTolerance && !isFacingRight) || (velocity.X < -FlipTolerance && isFacingRight)) {
				Flip();
			}

			var velocityXFactor = Mathf.Abs(velocity.X) / PhysicsPlayer.MaxHorizontalSpeed;
			var velocityYFactor =
				velocity.Y /
				(velocity.Y >= 0 ? PhysicsPlayer.MaxVerticalSpeed : -PhysicsPlayer.MinVerticalSpeed);

			if (animator != null) {
				animator.SetFloat("VelocityX", velocityXFactor);
				animator.SetFloat("VelocityY", velocityYFactor);
				animator.SetFloat("LandingVelocityYFactor", physicsPlayer.State.LandingVelocityYFactor);
				animator.SetBool("IsGroundSensorActive", physicsPlayer.State.IsGrounded);
				animator.SetBool("IsJumping", physicsPlayer.State.Animation == PlayerAnimation.Jumping);
				animator.SetBool("IsFalling", physicsPlayer.State.Animation == PlayerAnimation.Falling);
				animator.SetBool("IsLanding", physicsPlayer.State.Animation == PlayerAnimation.Landing);
			}
		}

		public void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
