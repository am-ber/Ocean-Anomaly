using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorState : State
{
	private BehavioralStateMachine stateManager;
	public BehaviorState(BehavioralStateMachine stateManager)
	{
		this.stateManager = stateManager;
	}
	public override void Initialize()
	{
		
	}

	public override void OnExit()
	{
		
	}

	public override void Update()
	{
		
	}
}
