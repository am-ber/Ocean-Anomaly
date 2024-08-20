using UnityEngine;

namespace OceanAnomaly.Components.Weapon
{
	[CreateAssetMenu(fileName = "DeafultWeapon", menuName = "ScriptableObjects/Weapon")]
	public class WeaponBaseScriptable : ScriptableObject
	{
		[Header("Identifying Variables")]
		public new string name;
		public string description;
		public Sprite weaponIcon;
		public Sprite weaponSprite;
		[Header("Attack Variables")]
		public ProjectileScriptable weaponFunction;
		public float attackTime = 1f;
	}
}