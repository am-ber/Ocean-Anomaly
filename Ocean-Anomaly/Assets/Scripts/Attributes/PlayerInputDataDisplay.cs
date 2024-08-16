using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public enum PlayerInputAction
{
	Up = '0',
	Right = '1',
	Down = '2',
	Left = '3',
	Attack = 'A',
	Dash = 'D',
}
[System.Serializable]
public struct PlayerInputData
{
	public PlayerInputAction action { get; }
	public bool held { get; }
	public double duration { get; }
	public PlayerInputData(PlayerInputAction action, bool held,double duration)
	{
		this.action = action;
		this.held = held;
		this.duration = duration;
	}
}
public class PlayerInputDataDisplay : MonoBehaviour
{
	public TextMeshProUGUI actionLetter;
	public TextMeshProUGUI actionName;
	public TextMeshProUGUI actionDuration;
	public PlayerInputData data;
	public void Set(PlayerInputData data)
	{
		this.data = data;
		actionLetter.text = "" + ((char)data.action);
		actionName.text = "" + data.action;
		actionDuration.text = "" + (data.duration == 0 ? "" : data.duration.ToString("F3"));
	}
}