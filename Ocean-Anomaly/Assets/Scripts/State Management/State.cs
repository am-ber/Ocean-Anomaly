using System;
using System.Collections;
using UnityEngine;

public abstract class State
{
	public bool Enabled = false;
	public abstract void Update();
	public virtual void OnEnter()
	{
		Enabled = true;
	}
	public virtual void OnExit()
	{
		Enabled = false;
	}
}
