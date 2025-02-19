using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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
public class Subject<I> : MonoBehaviour
{
	List<Observer<I>> observers = new List<Observer<I>>();
	public void AddObserver(Observer<I> observer)
	{
		observers.Add(observer);
	}
	public void RemoveObserver(Observer<I> observer)
	{
		observers.Remove(observer);
	}
	public void Notify(I input)
	{
		try
		{
			foreach (var observer in observers)
			{
				observer.OnNotify(input);
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Subject unable to notify observers because...\n {e.Message}");
		}
	}
}
public class Subject : MonoBehaviour
{
	List<Observer> observers = new List<Observer>();
	public void AddObserver(Observer observer)
	{
		observers.Add(observer);
	}
	public void RemoveObserver(Observer observer)
	{
		observers.Remove(observer);
	}
	public void Notify()
	{
		try
		{
			foreach (var observer in observers)
			{
				observer.OnNotify();
			}
		}
		catch (Exception e)
		{
			Debug.Log($"Subject unable to notify observers because...\n {e.Message}");
		}
	}
}