using System.Collections;
using UnityEngine;

public abstract class State
{
	private State previousState;
	public bool enabled = false;
	public abstract void Initialize();
	public abstract void Update();
	public virtual void OnEnter(State previousState)
	{
		this.previousState = previousState;
		enabled = true;
	}
	public State GetPrevious()
	{
		return previousState;
	}
	public virtual void OnExit()
	{
		enabled = false;
	}
}
