using GameLibrary;
using UnityEngine;
using Mathf = UnityEngine.Mathf;

namespace Scripts
{
	public class CameraController : MonoBehaviour
	{
		private new Camera camera;
		private PlayerView[] players;
		private Vector3 velocity = Vector3.zero;

		public float ViewportXTarget = 0.3f;
		public float DampTimeX = 0.4f;
		public float DampMaxSpeedX = 25f;
		public float ViewportYTarget = 0.35f;
		public float ViewportYMax = 0.64f;
		public float ViewportYMin = 0.22f;
		public float DampTimeY = 0.09f;
		public float DampMaxSpeedY = 1000f;

		public bool RequiredSmoothDamp { get; set; }

		public void Start()
		{
			camera = GetComponent<Camera>();
        }

		public void Awake()
		{
			players = FindObjectsOfType<PlayerView>();
		}

		public void Update()
		{
			if (velocity.magnitude <= 0.01f) {
				velocity = Vector3.zero;
			}

			transform.position = GetCameraClampedPosition();

			var cameraPosition = GetCameraPosition();
			transform.position =
				RequiredSmoothDamp ?
				new Vector3(
					Mathf.SmoothDamp(transform.position.x, cameraPosition.x, ref velocity.x, DampTimeX, DampMaxSpeedX),
					Mathf.SmoothDamp(transform.position.y, cameraPosition.y, ref velocity.y, DampTimeY, DampMaxSpeedY),
					cameraPosition.z
				) :
				cameraPosition;

			RequiredSmoothDamp = true;
		}

		private Vector3 GetCameraClampedPosition()
		{
			// TODO: Support of many players
			var player = players[0];

			var point = camera.WorldToViewportPoint(player.transform.position);

			// X target
			//var viewportXTarget = player.IsFacingRight ? ViewportXTarget : 1f - ViewportXTarget;
			var viewportXFraction = camera.WorldToViewportPoint(player.transform.position).x;
			//viewportXFraction = Mathf.Clamp(viewportXFraction, viewportXTarget - ViewportXTargetMaxInterval, viewportXTarget + ViewportXTargetMaxInterval);

			// Y target
			var viewportYFraction = camera.WorldToViewportPoint(player.transform.position).y;
			viewportYFraction = Mathf.Clamp(viewportYFraction, ViewportYMin, ViewportYMax);

			var delta = player.transform.position - camera.ViewportToWorldPoint(new Vector3(viewportXFraction, viewportYFraction, point.z));
			return transform.position + delta;
		}

		private Vector3 GetCameraPosition()
		{
			// TODO: Support of many players
			var player = players[0];

			var point = camera.WorldToViewportPoint(player.transform.position);

			// X target
			var viewportXTarget = (player.IsFacingRight && !player.State.IsClingedWall) || (!player.IsFacingRight && player.State.IsClingedWall) ? ViewportXTarget : 1f - ViewportXTarget;

			// Y target
			var viewportYTarget =
				(player.State.IsGrounded && player.State.Animation != PlayerAnimation.Jumping) || player.State.IsClingedWall ?
				ViewportYTarget :
				camera.WorldToViewportPoint(player.transform.position).y;

			var delta = player.transform.position - camera.ViewportToWorldPoint(new Vector3(viewportXTarget, viewportYTarget, point.z));
			return transform.position + delta;
		}
	}
}
