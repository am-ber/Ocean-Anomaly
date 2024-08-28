using OceanAnomaly.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Components
{
	[CreateAssetMenu(fileName = "UpgradeTree", menuName = "Scriptable Objects/Upgrades/Upgrade Tree")]
	public class UpgradeTreeScriptable : ScriptableObject
	{
		public new string name;
		public string description;
		public int treeStartingPoints = 0;
		[ReadOnly]
		[SerializeField]
		private int treePoints = 0;
		public int upgradeTreePoints { get; private set; }
		public List<UpgradeScriptable> upgradeList = new List<UpgradeScriptable>();
		private void OnValidate()
		{
			InitializeTree();
		}
		private void Awake()
		{
			InitializeTree();
		}
		public void InitializeTree()
		{
			treePoints = treeStartingPoints;
		}
		/// <summary>
		/// The int you give this will be ADDED to <seealso cref="treePoints"/>.
		/// </summary>
		/// <param name="points"></param>
		/// <param name="allowNegative"></param>
		public void AdjustTreePoints(int points, bool allowNegative = false)
		{
			// If we don't allow negative treePoints, and we can't upgrade with cost, then return.
			if (!allowNegative && !CanUpgradeWithCost(points))
			{
				return;
			}
			treePoints += points;
		}
		public int GetTreePoints()
		{
			return treePoints;
		}
		/// <summary>
		/// The int you check this with should be a positive value.
		/// </summary>
		/// <param name="upgradeCost"></param>
		/// <returns></returns>
		public bool CanUpgradeWithCost(int upgradeCost)
		{
			return (treePoints - upgradeCost) >= 0;
		}
	}
}
