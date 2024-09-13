using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class RigConstraintState : AnimationState
	{
		public Rig constrainedRig;
		public RigConstraintState(GameObject gameObject, AnimationDataScriptable animationData, Rig constrainedRig) : base(gameObject, animationData)
		{
			this.constrainedRig = constrainedRig;
		}
		public override void OnEnter()
		{
			base.OnEnter();
			constrainedRig.weight = animationData.activeWeight;
		}
		public override void OnExit()
		{
			base.OnExit();
			constrainedRig.weight = animationData.deactiveWeight;
		}
	}
}
