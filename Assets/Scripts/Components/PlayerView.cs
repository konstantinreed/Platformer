using GameLibrary;
using UnityEngine;

namespace Scripts
{
	using Mathf = GameLibrary.Mathf;

	public class PlayerView : MonoBehaviour
	{
		private const float FlipTolerance = 0.5f;
		private const float RotationSmoothing = 0.5f;

		private PhysicsPlayer physicsPlayer;
		private Animator animator;
		private Quaternion initialRotation;
		private bool isFacingRight = true;
		
		public GameObject AnimatorGameObject;
		public Transform RotationTransform;

		public void Start()
		{
			var unityApplication = UnityApplication.Instance;
			physicsPlayer = unityApplication.Client.Player;
			animator = AnimatorGameObject != null ? AnimatorGameObject.GetComponent<Animator>() : null;
			initialRotation = RotationTransform != null ? RotationTransform.rotation : Quaternion.identity;
		}

		public void Update()
		{
			transform.position = new Vector3(physicsPlayer.Body.Position.X, physicsPlayer.Body.Position.Y, 0f);

			var velocity = physicsPlayer.Body.LinearVelocity;

			if ((velocity.X > FlipTolerance && !isFacingRight) || (velocity.X < -FlipTolerance && isFacingRight)) {
				Flip();
			}

			var angle = Mathf.Lerp(RotationSmoothing, physicsPlayer.State.Rotation, 0f) * Mathf.RadToDeg;
			RotationTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * initialRotation;

			var velocityXFactor = Mathf.Abs(velocity.X) / PhysicsPlayer.MaxHorizontalSpeed;
			var velocityYFactor =
				velocity.Y /
				(velocity.Y >= 0 ? PhysicsPlayer.MaxVerticalSpeed : -PhysicsPlayer.MinVerticalSpeed);

			if (animator != null) {
				animator.SetFloat("VelocityX", velocityXFactor);
				animator.SetFloat("VelocityY", velocityYFactor);
				animator.SetFloat("Rotation", angle);
				animator.SetFloat("LandingVelocityYFactor", physicsPlayer.State.LandingVelocityYFactor);
				animator.SetBool("IsGroundSensorActive", physicsPlayer.State.IsGrounded);
				animator.SetBool("IsWallSensorActive", physicsPlayer.State.IsClingedWall);
				animator.SetBool(
					"IsIdle",
					physicsPlayer.State.Animation == PlayerAnimation.Idle ||
					physicsPlayer.State.Animation == PlayerAnimation.Running && velocityXFactor < 0.01f
				);
				animator.SetBool(
					"IsRunning",
					velocityXFactor >= 0.01f && physicsPlayer.State.Animation == PlayerAnimation.Running
				);
				animator.SetBool("IsJumping", physicsPlayer.State.Animation == PlayerAnimation.Jumping);
				animator.SetBool("IsWallJumping", physicsPlayer.State.Animation == PlayerAnimation.WallJumping);
				animator.SetBool("IsFalling", physicsPlayer.State.Animation == PlayerAnimation.Falling);
				animator.SetBool("IsWallFalling", physicsPlayer.State.Animation == PlayerAnimation.WallFalling);
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
