using System;
using System.Collections.Generic;
using UnityEngine;

public class Subject<I1, I2> : MonoBehaviour
{
	List<Observer<I1, I2>> observers = new List<Observer<I1, I2>>();
	public void AddObserver(Observer<I1, I2> observer)
	{
		observers.Add(observer);
	}
	public void RemoveObserver(Observer<I1, I2> observer)
	{
		observers.Remove(observer);
	}
	public void Notify(I1 caller, I2 input)
	{
		try
		{
			foreach (var observer in observers)
			{
				observer.OnNotify(caller, input);
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Subject unable to notify observers because...\n {e.Message}");
		}
	}
}