using OceanAnomaly.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Attributes
{
	public class FieldOfView : MonoBehaviour
	{
		public float viewRadius = 15;
		public float searchDelay = 0.1f;
		[Range(0, 360)]
		public float viewAngle = 0;
		public LayerMask targetMask;
		public LayerMask obstacleMask;
		[ReadOnly]
		public bool seeTarget = false;
		private bool seeTargetOneTime = true;
		[HideInInspector]
		public List<Transform> visibleTargets = new List<Transform>();

		private IEnumerator findTargetsCoroutine;

		private void OnEnable()
		{
			findTargetsCoroutine = FindTargetsWithDelay();
			StartCoroutine(findTargetsCoroutine);
		}

		IEnumerator FindTargetsWithDelay()
		{
			Debug.Log($"Looking for targets on layer: {LayerMask.LayerToName(GlobalTools.MaskToLayer(targetMask))}" +
				$" and avoiding layer: {LayerMask.LayerToName(GlobalTools.MaskToLayer(obstacleMask))}");
			while (true)
			{
				yield return new WaitForSeconds(searchDelay);
				FindVisibleTargets();
			}
		}
		void FindVisibleTargets()
		{
			visibleTargets.Clear();
			Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
			for (int i = 0; i < targetsInViewRadius.Length; i++)
			{
				Transform target = targetsInViewRadius[i].transform;
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
				{
					float dstToTarget = Vector3.Distance(transform.position, target.position);
					if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
					{
						visibleTargets.Add(target);
						if (seeTargetOneTime)
						{
							seeTarget = true;
							seeTargetOneTime = false;
						}
					}
				} else
				{
					seeTarget = false;
					seeTargetOneTime = true;
				}
			}
		}

		public Vector3 directionFromAngle(float angleInDegrees, bool angleIsGlobal)
		{
			if (!angleIsGlobal)
				angleInDegrees += transform.eulerAngles.z;
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
		}
		private void OnDisable()
		{
			StopCoroutine(findTargetsCoroutine);
		}
	}
}