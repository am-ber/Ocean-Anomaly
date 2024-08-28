using UnityEngine;

namespace OceanAnomaly.Components.Weapon
{
	[CreateAssetMenu(fileName = "DeafultWeapon", menuName = "Scriptable Objects/Weapon")]
	public class WeaponBaseScriptable : ScriptableObject
	{
		[Header("Identifying Variables")]
		public new string name;
		public string description;
		public Sprite weaponIcon;
		public Vector2 iconPositionOffset = Vector2.zero;
		public float iconAngleOffset = 0f;
		public Sprite weaponSprite;
		public string fireSoundName;
		[Header("Attack Variables")]
		public ProjectileScriptable weaponFunction;
		public float attackTime = 1f;
	}
}