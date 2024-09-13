using Dreamteck.Splines;
using OceanAnomaly.Attributes;
using OceanAnomaly.StateManagement;
using OceanAnomaly.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Controllers
{
	public class EnemyMovementController : MonoBehaviour
	{
		[Header("Movement Settings")]
		[SerializeField]
		private EnemyFieldManager fieldManager;
		[SerializeField]
		private StateManager stateManager;
		[ExposedScriptableObject]
		[SerializeField]
		private MovementDataScriptable wanderMovementData;
		[ExposedScriptableObject]
		[SerializeField]
		private MovementDataScriptable onTrackMovementData;
		[field: SerializeField]
		public WanderMovementState wanderState { get; private set; }
		public UnityEvent OnInitialized;
		private void Start()
		{
			// Checking for the Field manager
			if (fieldManager == null)
			{
				if (GlobalManager.Instance != null)
				{
					fieldManager = GlobalManager.Instance.enemyFieldManager;
				}
			}
			InitializeStates();
		}
		public void InitializeStates()
		{
			// Initialize States
			stateManager = new StateManager();
			wanderState = new WanderMovementState(transform, wanderMovementData);
			OnInitialized?.Invoke();
		}
		private void Update()
		{
			stateManager?.Update();
		}
		/// <summary>
		/// Changes the current <seealso cref="StateManager.CurrentState"/> with a specific MovementState.
		/// </summary>
		/// <param name="state"></param>
		public void ChangeMovementState(MovementState state)
		{
			stateManager.ChangeState(state);
		}
		/// <summary>
		/// Returns the current <seealso cref="State"/> that is hopefully the <seealso cref="MovementState"/> type from <seealso cref="StateManager.CurrentState"/>.
		/// </summary>
		/// <returns></returns>
		public MovementState GetMovementState()
		{
			try
			{
				return stateManager.GetCurrentState() as MovementState;
			}
			catch (Exception e)
			{
				print($"Couldn't return current EnemyMovement state because: {e.Message}\n{e.StackTrace}");
			}
			return null;
		}
	}
}