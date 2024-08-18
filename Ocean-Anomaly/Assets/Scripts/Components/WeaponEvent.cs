using OceanAnomaly.Controllers;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
	[CreateAssetMenu(fileName = "WeaponEvent", menuName = "ScriptableObjects/WeaponEvent")]
	public class WeaponEvent : ScriptableObject
	{
		public UnityAction<PlayerAttackController> AttackEvent;

		public void RaiseEvent(PlayerAttackController controller)
		{
			AttackEvent?.Invoke(controller);
		}
	}
}
