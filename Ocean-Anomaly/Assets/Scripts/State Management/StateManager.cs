using OceanAnomaly.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateManager
{
	public bool failedUpdateProtection = true;
	public readonly int maximumUpdateFailCount = 3;
	[ReadOnly]
	[SerializeField]
	private int currentUpdateFailure = 0;
	private State CurrentState;
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
		} catch (Exception e)
		{
			currentUpdateFailure++;
			Debug.Log($"Error updating in {ToString()}\t{maximumUpdateFailCount - currentUpdateFailure} attempts left.\n{e.StackTrace}");
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
