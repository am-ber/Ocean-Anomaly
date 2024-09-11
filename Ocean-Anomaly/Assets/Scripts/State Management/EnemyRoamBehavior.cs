using OceanAnomaly.Attributes;
using OceanAnomaly.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class EnemyRoamBehavior : BehaviorState
	{
		[SerializeField]
		private float reactionPercentage = 0.1f; // The closer to 1.0f (100%) the more aggressive to attacks the enemy will be.
		[SerializeField]
		private EnemyMovementController movementController;
		[SerializeField]
		private EnemyAIController aIController;
		[SerializeField]
		private EnemyFieldManager fieldManager;
		[ReadOnly]
		[SerializeField]
		private bool recievedDamage = false;
		public EnemyRoamBehavior(GameObject gameObject, BehaviorDataScriptable behaviorData, EnemyAIController aIController, EnemyMovementController movementController, EnemyFieldManager fieldManager) : base(gameObject, behaviorData)
		{
			this.movementController = movementController;
			this.fieldManager = fieldManager;
			this.aIController = aIController;
		}
		public override void OnEnter()
		{
			base.OnEnter();
			if (movementController == null)
			{
				movementController = gameObject.GetComponent<EnemyMovementController>();
			}
			// We need the MovementController
			movementController.ChangeMovementState(movementController.wanderState);
			movementController.GetMovementState().OnTargetReach.AddListener(OnTargetReachedRoaming);
			// But sometimes we might not have the fieldManager for some reason?
			if (fieldManager != null)
			{
				fieldManager.GenerateNewPoints();
				movementController.GetMovementState().SetTarget(fieldManager.GetFieldStart());
			}
		}
		public override void Update()
		{
			base.Update();
			// Check if we recieved damage and determine if we feel like changing states because of that.
			if (recievedDamage)
			{
				recievedDamage = false;
				if (WillReactToAttack(reactionPercentage))
				{
					aIController.ChangeEnemyState(aIController.huntingBehavior);
					return;
				}
			}
			// We now need to make sure we navigate to the start point of the Spline we generate in EnemyFieldManager.
		}
		public void OnRecieveDamage()
		{
			recievedDamage = true;
		}
		public void OnTargetReachedRoaming(Transform target)
		{
			Debug.Log($"{gameObject.name} reached {target.name}");
			movementController.ChangeMovementState(movementController.trackState);
			movementController.GetMovementState().OnTargetReach.RemoveListener(OnTargetReachedRoaming);
		}
		private bool WillReactToAttack(float percentage = 1f)
		{
			float likelyHood = UnityEngine.Random.Range(0f, 1f);
			if (likelyHood < percentage)
			{
				return true;
			}
			return false;
		}
	}
}
