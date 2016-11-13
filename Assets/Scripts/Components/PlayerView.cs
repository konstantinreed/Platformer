using GameLibrary;
using UnityEngine;

namespace Scripts
{
	using Mathf = GameLibrary.Mathf;
	using Vec2 = Microsoft.Xna.Framework.Vector2;

	public class PlayerView : MonoBehaviour
	{
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

			if ((velocity.X > 0 && !isFacingRight) || (velocity.X < 0 && isFacingRight)) {
				Flip();
			}

			var velocityXFactor = Mathf.Abs(velocity.X) / PhysicsPlayer.MaxHorizontalSpeed;
			var velocityYFactor =
				velocity.Y /
				(velocity.Y >= 0 ? PhysicsPlayer.MaxVerticalSpeed : -PhysicsPlayer.MinVerticalSpeed);

			if (animator != null) {
				animator.SetFloat("VelocityX", velocityXFactor);
				animator.SetFloat("VelocityY", velocityYFactor);
				//animator.SetFloat("LandingVelocityYFactor", landingVelocityYFactor);
				//animator.SetBool("IsGroundSensorActive", isOnGround);
				//animator.SetBool("IsJumping", state == State.Jumping);
				//animator.SetBool("IsFalling", state == State.Falling);
				//animator.SetBool("IsLanding", state == State.Landing);
			}
		}

		public void Flip()
		{
			isFacingRight = !isFacingRight;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
