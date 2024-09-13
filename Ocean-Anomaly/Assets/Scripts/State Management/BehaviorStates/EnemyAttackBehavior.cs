using OceanAnomaly.StateManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	[Serializable]
	public class EnemyAttackBehavior : BehaviorState
	{
		[SerializeField]
		private EnemyAIController aIController;
		[SerializeField]
		private EnemyMovementController movementController;
		public EnemyAttackBehavior(GameObject gameObject, BehaviorDataScriptable behaviorData, EnemyAIController aIController, EnemyMovementController movementController) : base(gameObject, behaviorData)
		{
			this.aIController = aIController;
			this.movementController = movementController;
		}
		public override void OnEnter()
		{
			base.OnEnter();
			// Code needed to run when we enter the state
		}
		public override void Update()
		{
			base.Update();
			// Code needed to run every update frame
		}
		public override void OnExit()
		{
			base.OnExit();
			// Code needed to run when we leave the state
		}
	}
}