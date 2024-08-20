using Cinemachine;
using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class PlayerVirtualCameraController : MonoBehaviour
	{
		[Header("Camera Target settings")]
		public List<Transform> CameraTargets = new List<Transform>();
		[SerializeField]
		private CinemachineVirtualCamera virtualCamera;
		[ReadOnly]
		[SerializeField]
		private Transform centralTarget;
		[ReadOnly]
		[SerializeField]
		private float greatestTargetDistance;
		[Header("Lens Settings")]
		[SerializeField]
		private float lensScaleMultiplier = 1.25f;
		[SerializeField]
		private float lensConstraintsMin = 5f;
		[SerializeField]
		private float lensConstraintsMax = 50f;
		[SerializeField]
		private float lensLerpScale = 2f;
		[ReadOnly]
		[SerializeField]
		private float lensTargetSize = 5f;
		[ReadOnly]
		[SerializeField]
		private float lensCurrentSize = 5f;

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
				if (CameraTargets[i] != null)
				{
					targetPositions[i] = CameraTargets[i].position;
				}
			}
			// Resolve the centroid and greatest distance
			Vector3 centerPoint = GlobalTools.FindCentroid(targetPositions);

			centralTarget.position = centerPoint;
			greatestTargetDistance = GlobalTools.FindLargestDistanceFromCentroid(targetPositions, centerPoint);

			virtualCamera.Follow = centralTarget;

			lensTargetSize = greatestTargetDistance * lensScaleMultiplier;
			lensCurrentSize = Mathf.Lerp(lensCurrentSize, lensTargetSize, lensLerpScale * Time.deltaTime);

			// Set the lens size to the greatest distance multiplied by some amount
			virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(lensCurrentSize, lensConstraintsMin, lensConstraintsMax);
		}
	}
}