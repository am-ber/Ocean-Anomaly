using Dreamteck.Splines;
using OceanAnomaly.Attributes;
using OceanAnomaly.StateManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Controllers
{
	public class EnemyAIController : MonoBehaviour
	{
		[Header("General AI Settings")]
		[SerializeField]
		private EnemyMovementController movementController;
		[SerializeField]
		private StateManager stateManager;
		[SerializeField]
		private EnemyFieldManager fieldManager;
		[Header("Behavior States")]
		[SerializeField]
		private BehaviorDataScriptable roamingData;
		public EnemyRoamBehavior roamBehavior;
		[SerializeField]
		private BehaviorDataScriptable huntingData;
		public EnemyHuntingBehavior huntingBehavior;
		[SerializeField]
		private BehaviorDataScriptable attackData;
		public EnemyAttackBehavior attackBehavior;
		[Header("Debug Settings")]
		[ReadOnly]
		[SerializeField]
		// This will be calculated by checking all possible limbs we have to attack with and their individual reach if the monster were to orient towards the player with that limb
		private float maximumReachDistance = 0f;
		private void Start()
		{
			Initialize();
		}
		private void Initialize()
		{
			// Grab components needed
			if (movementController == null)
			{
				movementController = GetComponent<EnemyMovementController>();
			}
			if (fieldManager == null)
			{
				if (GlobalManager.Instance != null)
				{
					fieldManager = GlobalManager.Instance.enemyFieldManager;
				}
			}
			// Initialize states
			stateManager = new StateManager();
			roamBehavior = new EnemyRoamBehavior(gameObject, roamingData, this, movementController, fieldManager);
			huntingBehavior = new EnemyHuntingBehavior(gameObject, huntingData);
			attackBehavior = new EnemyAttackBehavior(gameObject, attackData, this, movementController);
			// Setup initial enemy state
			movementController.OnInitialized.AddListener(() =>
			{
				ChangeEnemyState(roamBehavior);
			});
		}
		private void Update()
		{
			stateManager?.Update();
		}
		public void ChangeEnemyState(BehaviorState state)
		{
			stateManager.ChangeState(state);
		}
	}
}