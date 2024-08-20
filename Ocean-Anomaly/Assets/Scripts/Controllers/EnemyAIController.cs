using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class EnemyAIController : MonoBehaviour
	{
		[SerializeField]
		private MonsterMovement movement;
		[SerializeField]
		private FieldOfView fieldOfView; // Right now this ain't working because the Physics.SphereOverlap() isn't grabbing the right stuff.
		private void Awake()
		{
			if (movement == null)
			{
				movement = GetComponent<MonsterMovement>();
			}
			if (fieldOfView == null)
			{
				fieldOfView = GetComponent<FieldOfView>();
			}
		}

	}
}