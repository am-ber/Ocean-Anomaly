using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class AnimationState : State
	{
		public AnimationDataScriptable animationData;
		private GameObject gameObject;
		public AnimationState(GameObject gameObject, AnimationDataScriptable behaviorData)
		{
			this.gameObject = gameObject;
			this.animationData = behaviorData;
		}
		public override void OnEnter()
		{
			base.OnEnter();
			// This is where the code will go for using the transition time between animation states to do something
		}
		public override void Update()
		{
			
		}
	}
}