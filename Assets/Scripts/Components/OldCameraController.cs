using UnityEngine;

namespace Scripts
{
	public class OldCameraController : MonoBehaviour
	{
		private new Camera camera;
		//private Vector3 velocity = Vector3.zero;
		
		public GameObject FollowingObject;
		public float DampTime = 0.15f;

		public void Start()
		{
			camera = GetComponent<Camera>();
		}

		public void Update()
		{
			var point = camera.WorldToViewportPoint(FollowingObject.transform.position);
			var delta = FollowingObject.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			var destination = transform.position + delta;
			//transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, DampTime);
			transform.position = destination;
		}
	}
}
