using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Components {
	public class BodyPartSpawner : MonoBehaviour
	{
		[SerializeField]
		private BodyPart LimbPrefab;
		[SerializeField]
		private List<Transform> LimbPoints;
		[SerializeField]
		private int limbLengthLimit = 10;
		private void Start()
		{
			CreateLimbs();
		}
		public void CreateLimbs()
		{
			foreach (Transform limbTransform in LimbPoints)
			{
				if (limbTransform != null)
				{
					BodyPart previousPart = null;
					for (int i = 0; i < limbLengthLimit; i++)
					{
						if (previousPart == null)
						{
							previousPart = Instantiate(LimbPrefab, limbTransform);
						} else
						{
							BodyPart newPart = Instantiate(LimbPrefab, previousPart.EndPointOffset.position, previousPart.EndPointOffset.rotation);
							newPart.SetPreviousBodyPart(previousPart);
							previousPart = newPart;
						}
					}
				}
			}
		}
	}
}