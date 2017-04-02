using UnityEngine;

namespace Scripts
{
	public class CameraController : MonoBehaviour
	{
		private new Camera camera;
		private Vector3 velocity = Vector3.zero;

		public GameObject FollowingObject;
		public float DampTime = 0.09f;

		public void Start()
		{
			camera = GetComponent<Camera>();
			transform.position = GetCameraPosition();
        }

		public void Update()
		{
			transform.position = Vector3.SmoothDamp(transform.position, GetCameraPosition(), ref velocity, DampTime);
		}

		private Vector3 GetCameraPosition()
		{
			var point = camera.WorldToViewportPoint(FollowingObject.transform.position);
			var delta = FollowingObject.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			return transform.position + delta;
		}
	}
}
