using UnityEngine;

public class QuickFollowTarget : MonoBehaviour {

	[SerializeField]
	private Transform followTarget;
	[SerializeField]
	private float followSpeed = 0.5f;
	[SerializeField]
	private Vector3 offset = new Vector3(0, 2, 0);

	void FixedUpdate () {
		// Exit for no targets
		if (followTarget == null)
			return;
		// Takes the adjusted position and lerps to the target
		Vector3 adjustedPosition = followTarget.position + offset;
		transform.position = transform.position = Vector3.Lerp(transform.position, adjustedPosition, followSpeed);
	}
}
