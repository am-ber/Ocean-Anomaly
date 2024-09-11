using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSineMovement : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset = Vector3.zero;
	[SerializeField]
	private float timeStep = 1f;
	[SerializeField]
	private float waveSpeed = 1f;
	[Header("Debugging Variables")]
	[ReadOnly]
	[SerializeField]
	private float cycle = 0f;
	[ReadOnly]
	[SerializeField]
	private float currentSin = 0f;
	[ReadOnly]
	[SerializeField]
	private Vector3 velocity = Vector3.zero;
	[ReadOnly]
	[SerializeField]
	private Vector3 targetOffset = Vector3.zero;
	private void Update()
	{
		// Figure for the current cycle
		cycle += Time.deltaTime * waveSpeed;
		currentSin = Mathf.Sin(cycle);
		// Multiply the offset vector by sine over time
		targetOffset = new Vector3(currentSin * offset.x, currentSin * offset.y, currentSin * offset.z);
		Vector3 dampedTransform = Vector3.SmoothDamp(transform.position, targetOffset + transform.position, ref velocity, timeStep);
		// Apply to the transform
		transform.position = dampedTransform;
	}
}
