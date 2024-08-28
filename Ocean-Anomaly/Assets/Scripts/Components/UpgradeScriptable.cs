using OceanAnomaly.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
	public enum UpgradeRarity
	{
		Standard,
		Advanced,
		Supreme,
		Legendary
	}

	[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrades/Upgrade")]
	public class UpgradeScriptable : ScriptableObject
	{
		[Header("Individual Variables")]
		[ReadOnly]
		public SerializableGuid uuid;
		public new string name;
		public string Description;
		public Sprite DisplayIcon;
		public Vector2 IconOffset = Vector2.zero;
		public float IconAngleOffset = 0f;
		public bool Unlocked = false;
		public int UpgradeCost = 0;
		[Header("Relational Variables")]
		public UpgradeRarity PerceivedRarity = UpgradeRarity.Standard;
		public bool IsUnique = false;
		public float UpgradeValue = 0f;
		public UpgradeTreeScriptable UpgradeTree;
		public List<UpgradeScriptable> Prerequisites;
		public UnityEvent<float> UpgradeAction;
		public UpgradeScriptable()
		{
			uuid = Guid.NewGuid();
		}
		private void OnValidate()
		{
			
		}
		/// <summary>
		/// Checks all the UpgradeScriptables in Prerequisites to see if they are unlocked first.
		/// Admittedly, using <seealso cref="GetPrerequisitesStillNeeded()"/> and checking if that
		/// list is empty for seeing if this can be unlocked WHEN you plan to use that list would
		/// be more efficient than calling both methods and iterating through
		/// <seealso cref="Prerequisites"/> twice.
		/// </summary>
		/// <returns></returns>
		public bool IsSkillUnlockable()
		{
			// Run through all prerequisites and check for them being unlocked
			foreach (UpgradeScriptable upgrade in Prerequisites)
			{
				if (!upgrade.Unlocked)
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Returns a list of UpgradeScriptable that are still locked in the Prerequisites.
		/// </summary>
		/// <returns></returns>
		public List<UpgradeScriptable> GetPrerequisitesStillNeeded()
		{
			List<UpgradeScriptable> stillNeededPrerequisites = new List<UpgradeScriptable>();
			foreach (UpgradeScriptable upgrade in Prerequisites)
			{
				if (!upgrade.Unlocked)
				{
					stillNeededPrerequisites.Add(upgrade);
				}
			}
			return stillNeededPrerequisites;
		}
		/// <summary>
		/// Internally checks <seealso cref="IsSkillUnlockable()"/> to see if you actually can preform
		/// this action before toggling <seealso cref="Unlocked"/>.
		/// </summary>
		public bool Unlock()
		{
			// If we can't unlock the upgrade then lets back out
			if (!IsSkillUnlockable())
			{
				return false;
			}
			if (UpgradeTree != null)
			{
				if (UpgradeTree.CanUpgradeWithCost(UpgradeCost))
				{
					UpgradeTree.AdjustTreePoints(-UpgradeCost);
				} else
				{
					return false;
				}
			}
			// Check if the UpgradeAction isn't null and invoke, then set our unlocked to true.
			UpgradeAction?.Invoke(UpgradeValue);
			Unlocked = true;
			return true;
		}
		public override string ToString()
		{
			return $"{name}\n{Description}";
		}
	}
}