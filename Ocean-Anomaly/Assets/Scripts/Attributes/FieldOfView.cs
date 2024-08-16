using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Attributes
{
	public class FieldOfView : MonoBehaviour
	{

		public float viewRadius = 15;
		[Range(0, 360)]
		public float viewAngle = 0;

		public LayerMask targetMask;
		public LayerMask obstacleMask;

		[ReadOnly]
		public bool seeTarget = false;

		[HideInInspector]
		public List<Transform> visibleTargets = new List<Transform>();

		private void Start()
		{
			StartCoroutine("FindTargetsWithDelay", 0.1f);
		}

		IEnumerator FindTargetsWithDelay(float delay)
		{
			while (true)
			{
				yield return new WaitForSeconds(delay);
				FindVisibleTargets();
			}
		}

		private bool seeTargetOneTime = true;

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
				angleInDegrees += transform.eulerAngles.y;
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
	}
}