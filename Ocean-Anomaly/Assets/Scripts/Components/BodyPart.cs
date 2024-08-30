using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
	public class BodyPart : MonoBehaviour
	{
		[TagSelector]
		[SerializeField]
		private string endpointTag = "";
		[field: SerializeField]
		public Transform EndPointOffset {  get; private set; }
		[field: SerializeField]
		public BodyPart NextBodyPart {  get; private set; }
		public BodyPart PreviousBodyPart;
		public bool SnapToPrevious = true;
		[ReadOnly]
		public int LimbIndex = 0;
		[SerializeField]
		private GameObject destructionGFXPrefab;
		public UnityEvent OnDetatching;
		private void Awake()
		{
			Initialize();
		}
		private void OnValidate()
		{
			Initialize();
		}
		private void Initialize()
		{
			if (EndPointOffset == null)
			{
				EndPointOffset = transform.FindChildByTag(endpointTag);
			}
		}
		public void RemoveThisBodyPart()
		{
			// Check for the next body part in the chain so we can slide things down.
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
			// Check for our previous body part in the chain so we can set our next part if it's there.
			if (PreviousBodyPart != null)
			{
				if (NextBodyPart != null)
				{
					PreviousBodyPart.SetNextBodyPart(NextBodyPart);
				}
			}
			// Whenever we detatch, tell our subscribers that we did indeed detatch just now.
			OnDetatching?.Invoke();
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
		private void OnDestroy()
		{
			if (!gameObject.scene.isLoaded)
			{
				return;
			}
			if (destructionGFXPrefab != null)
			{
				Instantiate(destructionGFXPrefab, transform.position, transform.rotation);
			}
		}
	}
}