using OceanAnomaly.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class BehaviorState : State
	{
		public BehaviorDataScriptable behaviorData;
		[ReadOnly]
		[SerializeField]
		private double timeInState = 0f;
		[ReadOnly]
		[SerializeField]
		private float desiredTimeInState = 0f;
		[SerializeField]
		public UnityEvent OnEnabled;
		[SerializeField]
		public UnityEvent OnTimeInStateReached;
		[SerializeField]
		public UnityEvent OnDisabled;
		protected GameObject gameObject;
		public BehaviorState(GameObject gameObject, BehaviorDataScriptable behaviorData)
		{
			this.gameObject = gameObject;
			this.behaviorData = behaviorData;
		}
		public override void OnEnter()
		{
			base.OnEnter();
			desiredTimeInState = UnityEngine.Random.Range(behaviorData.MinTimeInState, behaviorData.MaxTimeInState);
			OnEnabled?.Invoke();
		}
		public override void Update()
		{
			timeInState += Time.deltaTime;
			if (timeInState >= desiredTimeInState)
			{
				OnTimeInStateReached?.Invoke();
			}
		}
		public override void OnExit()
		{
			base.OnExit();
			OnDisabled?.Invoke();
		}
	}
}