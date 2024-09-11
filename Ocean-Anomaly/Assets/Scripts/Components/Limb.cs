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
		public Transform EndPoint { get; private set; }
		public Vector3 EndPointOffset = Vector3.zero;
		public float LeftRightSeparation = 0f;
		[field: SerializeField]
		public Transform LeftPoint { get; private set; }
		[field: SerializeField]
		public Transform RightPoint { get; private set; }
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
			// Resolve limb outter points
			if (EndPoint == null)
			{
				EndPoint = transform.FindChildByTag(endpointTag);
				// If we didn't find an offset we can just make one
				if (EndPoint == null)
				{
					EndPoint = new GameObject($"{gameObject.name} End Point").transform;
					EndPoint.gameObject.tag = endpointTag;
					EndPoint.parent = transform;
				}
			}
			Vector3 midPoint = GlobalTools.MidPoint(transform.position, EndPoint.position);
			if (LeftPoint == null)
			{
				Vector3 leftPoint = transform.position.ToVector2().RotatePoint(midPoint, -90);
				LeftPoint = new GameObject($"{gameObject.name} Left Point").transform;
				LeftPoint.parent = transform;
				LeftPoint.position = leftPoint.AdjustDistance(midPoint, LeftRightSeparation);
			}
			if (RightPoint == null)
			{
				Vector3 rightPoint = transform.position.ToVector2().RotatePoint(midPoint, 90);
				RightPoint = new GameObject($"{gameObject.name} Right Point").transform;
				RightPoint.parent = transform;
				RightPoint.position = rightPoint.AdjustDistance(midPoint, LeftRightSeparation);
			}
			EndPoint.position += EndPointOffset;
			if (LimbHealth == null)
			{
				LimbHealth = gameObject.RecursiveFindComponentLocal<Health>();
			}
		}
		public void ResetLeftAndRightToSeparation()
		{
			// Solve for the mid point position
			Vector3 midPoint = GetMidPoint();
			// Solve for the left point
			Vector3 leftPoint = transform.position.ToVector2().RotatePoint(midPoint, -90).ToVector3().AdjustDistance(midPoint, LeftRightSeparation);
			LeftPoint.position = leftPoint;
			// Solve for the right point
			Vector3 rightPoint = transform.position.ToVector2().RotatePoint(midPoint, 90).ToVector3().AdjustDistance(midPoint, LeftRightSeparation);
			RightPoint.position = rightPoint;
		}
		public Vector3 GetMidPoint()
		{
			return GlobalTools.MidPoint(transform.position, EndPoint.position);
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