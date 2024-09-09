using Dreamteck.Splines;
using OceanAnomaly.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Controllers
{
	public enum BehaviorState
	{
		None,
		Roaming,
		Hunting,
		Attacking
	}
	public class EnemyAIController : MonoBehaviour
	{
		[SerializeField]
		private EnemyMovementController movementController;
		[SerializeField]
		public BehaviorState currentState = BehaviorState.None;
		[SerializeField]
		private EnemyFieldManager fieldManager;
		[ReadOnly]
		[SerializeField]
		private double timeInState = 0f;
		[ReadOnly]
		[SerializeField]
		private float timeToLeaveState = 0f;
		[ReadOnly]
		[SerializeField]
		private bool recievedDamage = false;
		// -------------------------------
		// I want to be clear... this entire section should be written in scriptable objects or at LEAST structs probably.
		// But I didn't have time to do that kind of structure. So this is what you get for now.
		// -------------------------------
		private Action currentStateAction;
		[Header("Roaming Settings")]
		[SerializeField]
		private float minTimeRoaming = 4f;
		[SerializeField]
		private float maxTimeRoaming = 10f;
		[SerializeField]
		private MovementData roamingMovement = new MovementData()
		{ MaxSpeed = 3f, MinSpeed = 1f, SteerStrength = 2f, WanderStrength = 0.5f };
		[SerializeField]
		private float reactionPercentage = 0.1f; // The closer to 1.0f (100%) the more aggressive to attacks the enemy will be.
		
		[Header("Hunting Settings")]
		[SerializeField]
		private float huntWanderStrength = 0.1f;
		[SerializeField]
		private MovementData huntingMovement = new MovementData()
		{ MaxSpeed = 5f, MinSpeed = 3f, SteerStrength = 3f, WanderStrength = 0.2f };
		[Header("Attacking Settings")]
		[SerializeField]
		private float minTimeAttacking = 4f;
		[SerializeField]
		private float maxTimeAttacking = 10f;
		[SerializeField]
		private MovementData attackMovement = new MovementData()
		{ MaxSpeed = 2f, MinSpeed = 0.5f, SteerStrength = 5f, WanderStrength = 0.05f };
		[ReadOnly]
		[SerializeField]
		// This will be calculated by checking all possible limbs we have to attack with and their individual reach if the monster were to orient towards the player with that limb
		private float maximumReachDistance = 0f;
		private void OnValidate()
		{
			Initialize();
		}
		private void Start()
		{
			Initialize();
		}
		private void Initialize()
		{
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
			ChangeEnemyState(currentState);
		}
		private void Update()
		{
			if (currentStateAction != null)
			{
				currentStateAction();
				timeInState += Time.deltaTime;
			}
		}
		private void OnRecieveDamage()
		{
			recievedDamage = true;
		}
		public void ChangeEnemyState(BehaviorState newState)
		{
			currentState = newState;
			switch (newState)
			{
				default:
					currentStateAction = null;
					break;
				case BehaviorState.Roaming:
					RoamingStateEnter();
					currentStateAction = RoamingState;
					break;
				case BehaviorState.Hunting:
					HuntingStateEnter();
					currentStateAction = HuntingState;
					break;
				case BehaviorState.Attacking:
					AttackingStateEnter();
					currentStateAction = AttackingState;
					break;
			}
		}
		private void RoamingStateEnter()
		{
			if (fieldManager != null)
			{
				fieldManager.GenerateNewPoints();
				movementController.SetTargetPosition(fieldManager.GetFieldStart());
			}
			if (movementController != null)
			{
				movementController.UpdateMovement(roamingMovement);
				movementController.SetMovementBehavior(MovementBehavior.WanderFollow);
				movementController.OnTargetReach.AddListener(OnTargetReachedRoaming);
			}
		}
		public void OnTargetReachedRoaming(Transform target)
		{
			print($"{name} reached {target.name}");
			movementController.SetMovementBehavior(MovementBehavior.OnTrack);
			movementController.OnTargetReach.RemoveListener(OnTargetReachedRoaming);
		}
		private void RoamingState()
		{
			// Check if we recieved damage and determine if we feel like changing states because of that.
			if (recievedDamage)
			{
				recievedDamage = false;
				if (WillReactToAttack(reactionPercentage))
				{
					ChangeEnemyState(BehaviorState.Hunting);
					return;
				}
			}
			// We now need to make sure we navigate to the start point of the Spline we generate in EnemyFieldManager.
			
		}
		private void HuntingStateEnter()
		{
			if (movementController != null)
			{
				movementController.UpdateMovement(huntingMovement);
			}
		}
		private void HuntingState()
		{

		}
		private void AttackingStateEnter()
		{
			if (movementController != null)
			{
				movementController.UpdateMovement(attackMovement);
			}
		}
		private void AttackingState()
		{
			if (movementController != null)
			{
				
			}

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