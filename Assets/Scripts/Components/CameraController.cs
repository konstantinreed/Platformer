using UnityEngine;

namespace Scripts
{
	public class CameraController : MonoBehaviour
	{
		private const float DampTimeX = 0.4f;
		private const float DampMaxSpeedX = 25f;
		private const float DampTimeY = 0.09f;
		private const float DampMaxSpeedY = 1000f;
		private const float ViewportYTarget = 0.35f;

		private new Camera camera;
		private PlayerView[] players;
		private Vector3 velocity = Vector3.zero;

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
			var cameraPosition = GetCameraPosition();
			transform.position =
				RequiredSmoothDamp ?
				new Vector3(
					Mathf.SmoothDamp(transform.position.x, cameraPosition.x, ref velocity.x, DampTimeX, DampMaxSpeedX),
					Mathf.SmoothDamp(transform.position.y, cameraPosition.y, ref velocity.y, DampTimeY, DampMaxSpeedY),
					cameraPosition.z
				) :
				//Vector3.SmoothDamp(transform.position, cameraPosition, ref velocity, DampTime) :
				cameraPosition;
			RequiredSmoothDamp = true;
		}

		private Vector3 GetCameraPosition()
		{
			// TODO: Support of many players
			var player = players[0];

			var point = camera.WorldToViewportPoint(player.transform.position);
			var viewportXTarget = player.IsFacingRight ? 0.3f : 0.7f;
			var delta = player.transform.position - camera.ViewportToWorldPoint(new Vector3(viewportXTarget, ViewportYTarget, point.z));
			return transform.position + delta;
		}
	}
}
