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
		private string endpointTag = "";
		[field: SerializeField]
		public Transform EndPointOffset {  get; private set; }
		[field: SerializeField]
		public Limb NextBodyPart {  get; private set; }
		[field: SerializeField]
		public Limb PreviousBodyPart { get; private set; }
		[ReadOnly]
		public int LimbIndex = 0;
		public UnityEvent<Limb> OnDetatching;
		
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
		public void SnapToPrevious()
		{
			if (PreviousBodyPart == null)
			{
				transform.position = transform.parent.position;
			} else
			{
				transform.position = PreviousBodyPart.EndPointOffset.position;
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
			OnDetatching?.Invoke(this);
		}
		/// <summary>
		/// Sets the NextBodyPart, the NextBodyPart's PreviousBodyPart reference, and the parent of the NextBodyPart transform.
		/// </summary>
		/// <param name="limbPart"></param>
		public void SetNextBodyPart(Limb limbPart)
		{
			NextBodyPart = limbPart;
			NextBodyPart.PreviousBodyPart = this;
			NextBodyPart.transform.parent = transform;
		}
		/// <summary>
		/// Sets the PreviousBodyPart, the PreviousBodyPart's NextBodyPart reference, and the parent of this transform.
		/// </summary>
		/// <param name="limbPart"></param>
		public void SetPreviousBodyPart(Limb limbPart)
		{
			PreviousBodyPart = limbPart;
			PreviousBodyPart.NextBodyPart = this;
			transform.parent = PreviousBodyPart.transform;
		}
	}
}