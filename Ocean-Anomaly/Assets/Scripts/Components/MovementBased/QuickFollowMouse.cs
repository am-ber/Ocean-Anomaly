using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using UnityEngine;

public class QuickFollowMouse : MonoBehaviour {

	[SerializeField]
	private float followSpeed = 0.5f;
	[SerializeField]
	private Vector3 offset = new Vector3(0, 2, 0);
	[ReadOnly]
	[SerializeField]
	private Vector3 currentVelocity = Vector3.zero;
	private void Update () {
		// Get mouse position in 2D only
		Vector2 worldPoint2d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 adjustedPosition = worldPoint2d.ToVector3() + offset;
		transform.position = transform.position = Vector3.SmoothDamp(transform.position, adjustedPosition, ref currentVelocity, followSpeed);
	}
}
