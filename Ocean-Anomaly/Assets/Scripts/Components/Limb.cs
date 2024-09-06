using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace OceanAnomaly.Components
{
	public class Limb : MonoBehaviour
	{
		[TagSelector]
		[SerializeField]
		public string endpointTag = "Untagged";
		[field: SerializeField]
		public Transform EndPointOffset { get; private set; }
		public Vector3 EndPointPositionOffset = Vector3.zero;
		[field: SerializeField]
		public Limb NextBodyPart { get; private set; }
		[field: SerializeField]
		public Limb PreviousBodyPart { get; private set; }
		public Health LimbHealth;
		public Transform parent;
		[ReadOnly]
		public int LimbIndex = 0;
		[ReadOnly]
		public object lockObject = new object();
		public UnityEvent<Limb> OnDetatchingEntry;
		public UnityEvent<Limb> OnDetatchingExit;

		private void Awake()
		{
			Initialize();
		}
		private void Initialize()
		{
			if (EndPointOffset == null)
			{
				EndPointOffset = transform.FindChildByTag(endpointTag);
				// If we didn't find an offset we can just make one
				if (EndPointOffset == null)
				{
					EndPointOffset = new GameObject($"{gameObject.name} End Point").transform;
					EndPointOffset.gameObject.tag = endpointTag;
					EndPointOffset.transform.parent = transform;
				}
			}
			EndPointOffset.position += EndPointPositionOffset;
			if (LimbHealth == null)
			{
				LimbHealth = gameObject.RecursiveFindComponentLocal<Health>();
			}
		}
		public void SnapToPrevious()
		{
			transform.position = parent.position;
		}
		public void RemoveThisBodyPart()
		{
			OnDetatchingEntry?.Invoke(this);
			lock (lockObject)
			{
				// Check for the next body part in the chain so we can slide things down.
				if (NextBodyPart != null)
				{
					if (PreviousBodyPart != null)
					{
						NextBodyPart.SetPreviousBodyPart(PreviousBodyPart);
					} else
					{
						NextBodyPart.parent = parent;
						NextBodyPart.transform.parent = parent;
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
			}
			// Whenever we detatch, tell our subscribers that we did indeed detatch just now.
			OnDetatchingExit?.Invoke(this);
		}
		/// <summary>
		/// Sets the NextBodyPart, the NextBodyPart's PreviousBodyPart reference, and the parent of the NextBodyPart transform.
		/// </summary>
		/// <param name="limbPart"></param>
		public void SetNextBodyPart(Limb limbPart)
		{
			lock (lockObject)
			{
				NextBodyPart = limbPart;
				NextBodyPart.PreviousBodyPart = this;
				NextBodyPart.transform.parent = transform;
			}
		}
		/// <summary>
		/// Sets the PreviousBodyPart, the PreviousBodyPart's NextBodyPart reference, and the parent of this transform.
		/// </summary>
		/// <param name="limbPart"></param>
		public void SetPreviousBodyPart(Limb limbPart)
		{
			lock (lockObject)
			{
				PreviousBodyPart = limbPart;
				PreviousBodyPart.NextBodyPart = this;
				parent = PreviousBodyPart.transform;
				transform.parent = parent;
			}
		}
	}
}