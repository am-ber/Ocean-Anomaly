using UnityEngine;
using UnityEngine.Events;

public abstract class State
{
	private State previousState;
	public abstract void Initialize();
	public abstract void Update();
	public virtual void OnEnter(State previousState)
	{
		this.previousState = previousState;
	}
	public State GetPrevious()
	{
		return previousState;
	}
	public abstract void OnExit();
}
