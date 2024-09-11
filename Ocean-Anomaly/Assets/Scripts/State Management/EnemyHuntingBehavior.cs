using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.StateManagement
{
	public class EnemyHuntingBehavior : BehaviorState
	{
		public EnemyHuntingBehavior(GameObject gameObject, BehaviorDataScriptable behaviorData) : base(gameObject, behaviorData)
		{
		}
	}
}
