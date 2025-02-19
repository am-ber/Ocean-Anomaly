using Dreamteck.Splines;
using OceanAnomaly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class OnTrackMovement : MovementState
	{
		[SerializeField]
		private SplineComputer splineComputer;
		[SerializeField]
		private SplineFollower splineFollower;
		public OnTrackMovement(Transform transform, MovementDataScriptable movementData, SplineComputer splineComputer, SplineFollower splineFollower = null) : base(transform, movementData)
		{
			if (splineFollower == null)
			{
				splineFollower = transform.GetComponent<SplineFollower>();
				splineFollower.wrapMode = SplineFollower.Wrap.Loop;
				splineFollower.motion.rotationOffset = new Vector3(0, 0, 180);
				splineFollower.followSpeed = movementData.MaxSpeed;
			}
			this.splineComputer = splineComputer;
			this.splineFollower = splineFollower;
		}
		public override void OnEnter()
		{
			base.OnEnter();
			splineComputer?.Subscribe(splineFollower);
			splineFollower.spline = splineComputer;
		}
		public override void OnExit()
		{
			base.OnExit();
			splineComputer?.Unsubscribe(splineFollower);
			splineFollower.spline = null;
		}
	}
}