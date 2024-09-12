using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/States/Behavior Data")]
	public class BehaviorDataScriptable : ScriptableObject
	{
		public float MaxTimeInState = 10f;
		public float MinTimeInState = 5f;
	}
}
