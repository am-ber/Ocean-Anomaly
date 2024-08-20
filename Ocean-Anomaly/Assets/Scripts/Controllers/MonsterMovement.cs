using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OceanAnomaly.Attributes;
using Unity.Mathematics;
using OceanAnomaly.Tools;
using OceanAnomaly.Components;

namespace OceanAnomaly.Controllers
{
	public enum MovementBehavior
	{
		Idle,
		OnTrack,
		Moving,
		Strafing
	}
	public class MonsterMovement : BasicMoveBehavior
	{
		[Header("Time Properties")]
		[SerializeField]
		private float timeSlow = 0.01f;
		[SerializeField]
		private float timeFast = 0.05f;
		[SerializeField]
		[ReadOnly]
		private float waitCount = 0f;
		[SerializeField]
		private float waitDuration = 2.5f;

		[Header("Movement Properties")]
		public bool isSwaying = false;
		[SerializeField]
		private float tailAngleRangeAllowed = 0.1f;
		[ReadOnly]
		public float tiltDirection = 0f;
		[ReadOnly]
		[SerializeField]
		private float headAnglePrevious = 0f;
		[ReadOnly]
		[SerializeField]
		private float headAngleDelta = 0f;
		[ReadOnly]
		[SerializeField]
		private float animationDelta = 0f;
		[SerializeField]
		private float headAngleMaxDelta = 2f;
		public MovementBehavior moveBehavior;
		[ReadOnly]
		public bool atTargetPosition = false;
		[SerializeField]
		private float minimumTargetDistance = 3f;
		[ReadOnly]
		[SerializeField]
		private float distanceTillTarget = 0f;

		void Update()
		{
			AnimateTail();
			if (moveBehavior == MovementBehavior.Moving)
			{
				rotateTo();
				CheckIfAtMoveTarget();
			}
		}
		private void FixedUpdate()
		{
			if (moveBehavior == MovementBehavior.Moving)
			{
				FixedBasicMove();
			}
		}
		private void CheckIfAtMoveTarget()
		{
			distanceTillTarget = Vector3.Distance(transform.position, targetPosition);
			if (distanceTillTarget <= minimumTargetDistance)
			{
				atTargetPosition = true;
				moveBehavior = MovementBehavior.Idle;
			} else
			{
				atTargetPosition = false;
			}
		}
		private void AnimateTail()
		{
			float headAngle = 0f;

			headAngle = gameObject.transform.eulerAngles.z;

			if (GlobalTools.InRange(headAngle, headAnglePrevious - tailAngleRangeAllowed, headAnglePrevious + tailAngleRangeAllowed))
			{
				waitCount += Time.deltaTime;
				if (waitCount < waitDuration)
				{
					tiltDirection = Mathf.Lerp(tiltDirection, 0f, timeSlow);
					isSwaying = false;
				} else
				{
					isSwaying = true;
				}
			} else
			{
				// Resolve the distance from our previous angle to the current one
				headAngleDelta = headAnglePrevious - headAngle;
				// Find the mapped delta to the animation curve
				animationDelta = GlobalTools.Map(
					Mathf.Clamp(headAngleDelta, -headAngleMaxDelta, headAngleMaxDelta),
					-headAngleMaxDelta, headAngleMaxDelta, -1, 1);
				// Set the tilt direction
				tiltDirection = Mathf.Lerp(tiltDirection, animationDelta, timeFast);
				isSwaying = false;
				waitCount = 0f;
			}

			headAnglePrevious = headAngle;
		}
	}
}