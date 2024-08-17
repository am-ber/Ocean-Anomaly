using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public enum PlayerInputAction
{
	None = -1,
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
	public PlayerInputAction Action { get; }
	public bool Held { get; }
	public double Duration { get; }
	public PlayerInputData(PlayerInputAction action, bool held, double duration)
	{
		Action = action;
		Held = held;
		Duration = duration;
	}
}
public class PlayerInputDataDisplay : MonoBehaviour
{
	public TextMeshProUGUI ActionLetter;
	public TextMeshProUGUI ActionName;
	public TextMeshProUGUI ActionDuration;
	public PlayerInputData Data;
	public void Set(PlayerInputData data)
	{
		Data = data;
		ActionLetter.text = "" + ((char)data.Action);
		ActionName.text = "" + data.Action;
		ActionDuration.text = "" + (data.Duration == 0 ? "" : data.Duration.ToString("F3"));
	}
}