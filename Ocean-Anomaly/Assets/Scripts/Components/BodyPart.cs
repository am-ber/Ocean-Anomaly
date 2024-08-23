using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.Components
{
	public class BodyPart : MonoBehaviour
	{
		[TagSelector]
		public string AllowedLimbPointTag;
		public Transform EndPointOffset;
		public BodyPart NextBodyPart;
		public BodyPart PreviousBodyPart;
		public bool SnapToPrevious = true;
		private void Update()
		{
			if (SnapToPrevious)
			{
				if (PreviousBodyPart != null)
				{
					transform.position = PreviousBodyPart.EndPointOffset.position;
					PreviousBodyPart.transform.parent = transform;
				}
			}
		}
		public void RemoveThisBodyPart()
		{
			// Check for the next body part in the chain so we can slide things down
			if (NextBodyPart != null)
			{
				if (PreviousBodyPart != null)
				{
					NextBodyPart.SetPreviousBodyPart(PreviousBodyPart);
				} else
				{
					NextBodyPart.transform.parent = transform.parent;
				}
			}
			if (PreviousBodyPart != null)
			{
				if (NextBodyPart != null)
				{
					PreviousBodyPart.SetNextBodyPart(NextBodyPart);
				}
			}
		}
		public void SetNextBodyPart(BodyPart bodyPart)
		{
			NextBodyPart = bodyPart;
			NextBodyPart.PreviousBodyPart = this;
			NextBodyPart.transform.parent = transform;
		}
		public void SetPreviousBodyPart(BodyPart bodyPart)
		{
			PreviousBodyPart = bodyPart;
			PreviousBodyPart.NextBodyPart = this;
			transform.parent = PreviousBodyPart.transform;
		}
	}
}