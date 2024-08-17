using Cinemachine;
using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVirtualCameraController : MonoBehaviour
{
	public List<Transform> CameraTargets = new List<Transform>();
	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;
	[ReadOnly]
	[SerializeField]
	private Transform centralTarget;
	[ReadOnly]
	[SerializeField]
	private float greatestTargetDistance;
	private void Awake()
	{
		if (virtualCamera == null)
		{
			virtualCamera = GetComponent<CinemachineVirtualCamera>();
		}
		if (centralTarget == null)
		{
			GameObject emptyObject = new GameObject("CameraCentroid");
			centralTarget = emptyObject.transform;
		}
	}
	private void LateUpdate()
	{
		// Iterate through the camera targets to gather their position vectors
		Vector3[] targetPositions = new Vector3[CameraTargets.Count];
		for (int i = 0; i < CameraTargets.Count; ++i)
		{
			targetPositions[i] = CameraTargets[i].position;
		}
		// Resolve the centroid and greatest distance
		Vector3 centerPoint = GlobalTools.FindCentroid(targetPositions);

		centralTarget.position = centerPoint;
		greatestTargetDistance = GlobalTools.FindLargestDistanceFromCentroid(targetPositions, centerPoint);

		virtualCamera.Follow = centralTarget;
	}
}
