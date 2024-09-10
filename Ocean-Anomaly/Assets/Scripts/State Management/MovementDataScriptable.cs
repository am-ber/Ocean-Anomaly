using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/States/Movement Data")]
public class MovementDataScriptable : ScriptableObject
{
	public float MaxSpeed;
	public float MinSpeed;
	public float SteerStrength;
	public float WanderStrength;
	public bool RemoveTargetAfterReach;
}
