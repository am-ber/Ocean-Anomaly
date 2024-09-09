using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
	[ReadOnly]
	[SerializeField]
	protected State CurrentState;
	void Update()
	{
		CurrentState?.Update();
	}
	public void ChangeState(State state)
	{
		if ((CurrentState == state) || (state == null))
		{
			return;
		}
		CurrentState?.OnExit();
		state.OnEnter(CurrentState);
		CurrentState = state;
	}
	public State GetCurrentState()
	{
		return CurrentState;
	}
}
