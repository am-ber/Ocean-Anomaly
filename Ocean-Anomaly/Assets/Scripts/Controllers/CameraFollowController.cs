using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class CameraFollowController : MonoBehaviour
	{
		public Transform followTarget;
		public float followSpeed = 0.5f;
		public float followHeight = -6f;
		void Update()
		{
			if (followTarget == null)
				return;
			// Camera position
			Vector3 adjustedHeight = followTarget.position + new Vector3(0, followHeight, 0);
			transform.position = Vector3.Lerp(transform.position, adjustedHeight, followSpeed);
			// Camera rotation
			transform.LookAt(followTarget, transform.TransformDirection(Vector3.up));
		}
	}
}