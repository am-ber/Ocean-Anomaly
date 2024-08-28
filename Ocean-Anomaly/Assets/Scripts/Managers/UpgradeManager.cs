using System;
using System.Collections.Generic;
using OceanAnomaly.Components;
using UnityEngine;

namespace OceanAnomaly.Managers
{
	public static class UpgradeManager
	{
		// Movement specific
		public static float dashTime = 1;			// smaller for better
		public static float moveFactor = 1;			// larger for better
		// Attack specific
		public static float projectileSpeed = 1;	// larger for better
		public static float attackTime = 1;			// smaller for better
		public static float projectileAccuracy = 1; // smaller for better
	}
	[CreateAssetMenu(fileName = "PlayerUpgradeManager", menuName = "Scriptable Objects/Upgrades/Player Upgrade Manager")]
	public class PlayerUpgradeManagerScriptable : ScriptableObject
	{
		[Header("Movement Specific")]
		public float dashTime = 1;           // smaller for better
		public float moveFactor = 1;         // larger for better
		[Header("Attack Specific")]
		public float projectileSpeed = 1;    // larger for better
		public float attackTime = 1;         // smaller for better
		public float projectileAccuracy = 1; // smaller for better
		public void IncreaseProjectileSpeed(float value)
		{
			projectileSpeed += value;
		}
	}
}
