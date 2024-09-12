using OceanAnomaly.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class StateManager
	{
		public bool failedUpdateProtection = true;
		[SerializeField]
		public int maximumUpdateFailCount = 3;
		[ReadOnly]
		[SerializeField]
		private int currentUpdateFailure = 0;
		private State CurrentState;
		public UnityEvent OnMaxFailedUpdates;
		public void Update()
		{
			if (currentUpdateFailure >= maximumUpdateFailCount)
			{
				return;
			}
			try
			{
				if (CurrentState != null && CurrentState.Enabled)
				{
					CurrentState.Update();
				}
			}
			catch (Exception e)
			{
				currentUpdateFailure++;
				Debug.Log($"Error updating in {ToString()}\t{maximumUpdateFailCount - currentUpdateFailure} attempts left.\n{e.StackTrace}");
				if (currentUpdateFailure >= maximumUpdateFailCount)
				{
					OnMaxFailedUpdates?.Invoke();
				}
			}
		}
		public void ChangeState(State state)
		{
			if ((CurrentState == state) || (state == null))
			{
				return;
			}
			Debug.Log($"Entering {state}");
			CurrentState?.OnExit();
			currentUpdateFailure = 0;
			state.OnEnter();
			CurrentState = state;
		}
		public State GetCurrentState()
		{
			return CurrentState;
		}
	}
}