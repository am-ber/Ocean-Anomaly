using System;
using System.Collections;
using System.Collections.Generic;
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
		public Action attackAction;
		public float attackTime = 1f;
		public WeaponType weaponType = WeaponType.Empty;
	}
}