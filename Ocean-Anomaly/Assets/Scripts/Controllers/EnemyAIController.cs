using Dreamteck.Splines;
using OceanAnomaly.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		private MonsterMovement movement;
		[SerializeField]
		public BehaviorState currentState = BehaviorState.None;
		[SerializeField]
		private EnemyFieldManager fieldManager;
		[SerializeField]
		private SplineFollower splineFollower;
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
		private float roamSpeed = 3f;
		[SerializeField]
		private float reactionPercentage = 0.1f; // The closer to 1.0f (100%) the more aggressive to attacks the enemy will be.
		private bool justEnteredRoamState = true;
		
		[Header("Hunting Settings")]
		[ReadOnly]
		[SerializeField]
		private bool travelDirectlyToPlayer = true; // Keep this checked until we have more random monster behavior
		[Header("Attacking Settings")]
		[SerializeField]
		private float minTimeAttacking = 4f;
		[SerializeField]
		private float maxTimeAttacking = 10f;
		private void Start()
		{
			if (movement == null)
			{
				movement = GetComponent<MonsterMovement>();
			}
			if (fieldManager == null)
			{
				fieldManager = GlobalManager.Instance.enemyFieldManager;
			}
			if (splineFollower == null)
			{
				splineFollower = GetComponent<SplineFollower>();
			}
		}
		private void OnEnable()
		{
			splineFollower.followSpeed = roamSpeed;
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
					currentStateAction = RoamingState;
					break;
				case BehaviorState.Hunting:
					currentStateAction = HuntingState;
					break;
				case BehaviorState.Attacking:
					currentStateAction = AttackingState;
					break;
			}
		}
		private void RoamingState()
		{
			// Check if we recieved damage and determine if we feel like changing states because of that.
			if (recievedDamage)
			{
				recievedDamage = false;
				if (WillReactToAttack(reactionPercentage))
				{
					justEnteredRoamState = true;
					fieldManager.UnsubscribeToSpline(splineFollower);
					ChangeEnemyState(BehaviorState.Hunting);
					return;
				}
			}
			// We now need to make sure we navigate to the start point of the Spline we generate in EnemyFieldManager.
			// Lets make sure we don't repeatedly generate new points to traverse.
			if (justEnteredRoamState)
			{
				fieldManager.GenerateNewPoints();
				justEnteredRoamState = false;
			}

		}
		private void HuntingState()
		{

		}
		private void AttackingState()
		{

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