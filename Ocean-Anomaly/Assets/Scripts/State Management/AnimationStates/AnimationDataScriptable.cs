using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/States/Animation Data")]
	public class AnimationDataScriptable : ScriptableObject
	{
		public float transitionTime = 1f;
		public float activeWeight = 1f;
		public float deactiveWeight = 0f;
	}
}
