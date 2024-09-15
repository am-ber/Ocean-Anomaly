using OceanAnomaly.Animation;
using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class LimbController : MonoBehaviour
	{
		[ReadOnly]
		[SerializeField]
		public float limbTotalHealth = 0f;
		[SerializeField]
		protected LimbDrawer limbDrawer;
		private void Awake()
		{
			if (limbDrawer == null)
			{
				limbDrawer = GetComponent<LimbDrawer>();
			}
		}
	}
}
