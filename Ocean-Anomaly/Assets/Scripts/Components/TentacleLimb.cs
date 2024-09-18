using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
	public class TentacleLimb : MonoBehaviour
	{
		[TagSelector]
		[SerializeField]
		public string endpointTag = "Untagged";
		public Health LimbHealth;
		public SpriteRenderer LimbGfx;
		[Header("Limb Extent References")]
		public float LeftRightSeparation = 0f;
		[field: SerializeField]
		public Transform EndPoint { get; private set; }
		[field: SerializeField]
		public Transform LeftPoint { get; private set; }
		[field: SerializeField]
		public Transform RightPoint { get; private set; }
		[Header("Limb Linking Variables")]
		public Transform parent;
		[field: SerializeField]
		public TentacleLimb NextBodyPart { get; private set; }
		[field: SerializeField]
		public TentacleLimb PreviousBodyPart { get; private set; }
		public UnityEvent<TentacleLimb> OnDetatchingEntry;
		public UnityEvent<TentacleLimb> OnDetatchingExit;
		[ReadOnly]
		public object lockObject = new object();
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
			// Find local needed components
			if (LimbHealth == null)
			{
				LimbHealth = gameObject.RecursiveFindComponentLocal<Health>();
			}
			if (LimbGfx == null)
			{
				LimbGfx = gameObject.RecursiveFindComponentLocal<SpriteRenderer>();
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
			if (PreviousBodyPart != null)
			{
				transform.position = PreviousBodyPart.EndPoint.position;
			} else
			{
				transform.position = parent.position;
			}
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
		public void SetNextBodyPart(TentacleLimb limbPart)
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
		public void SetPreviousBodyPart(TentacleLimb limbPart)
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