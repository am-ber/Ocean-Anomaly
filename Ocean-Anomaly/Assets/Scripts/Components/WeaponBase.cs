using OceanAnomaly.Controllers;
using UnityEngine;

namespace OceanAnomaly.Components
{
	public enum WeaponType
	{
		Empty,
		Melee,
		Ranged,
		Special
	}
	[CreateAssetMenu(fileName = "DeafultWeapon", menuName = "ScriptableObjects/Weapon")]
	public class WeaponBase : ScriptableObject
	{
		[Header("Identifying Variables")]
		public new string name;
		public string description;
		public Sprite weaponIcon;
		public Sprite weaponSprite;
		[Header("Attack Variables")]
		public WeaponEvent weaponEvent;
		public float attackTime = 1f;
		public WeaponType weaponType = WeaponType.Empty;

		public void TriggerWeaponEvent(PlayerAttackController attackController)
		{
			if (weaponEvent != null)
			{
				weaponEvent.RaiseEvent(attackController);
			}
		}
	}
}