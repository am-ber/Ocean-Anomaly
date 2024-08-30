using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

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
		public UnityEvent OnDetatching;
		[Header("Graphics and other GFX Variables")]
		[ColorUsage(true, true)]
		[SerializeField]
		private Color onHealColor = Color.green;
		[ColorUsage(true, true)]
		[SerializeField]
		private Color onHitColor = Color.red;
		[SerializeField]
		private SpriteRenderer graphicsRenderer;
		[SerializeField]
		private GameObject destructionGFXPrefab;
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
			if (graphicsRenderer == null)
			{
				graphicsRenderer = gameObject.RecursiveFindComponentLocal<SpriteRenderer>();
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
		/// <summary>
		/// Sets the NextBodyPart, the NextBodyPart's PreviousBodyPart reference, and the parent of the NextBodyPart transform.
		/// </summary>
		/// <param name="bodyPart"></param>
		public void SetNextBodyPart(BodyPart bodyPart)
		{
			NextBodyPart = bodyPart;
			NextBodyPart.PreviousBodyPart = this;
			NextBodyPart.transform.parent = transform;
		}
		/// <summary>
		/// Sets the PreviousBodyPart, the PreviousBodyPart's NextBodyPart reference, and the parent of this transform.
		/// </summary>
		/// <param name="bodyPart"></param>
		public void SetPreviousBodyPart(BodyPart bodyPart)
		{
			PreviousBodyPart = bodyPart;
			PreviousBodyPart.NextBodyPart = this;
			transform.parent = PreviousBodyPart.transform;
		}
		public void OnDamage(float amount)
		{
			// If we don't have a reference to a renderer then lets just return
			if (graphicsRenderer == null)
			{
				return;
			}
			// If we are taking damage
			if (amount < 0)
			{
				float tweenTime = 0.1f;
				Color previousColor = graphicsRenderer.color;
				graphicsRenderer.DOColor(onHitColor, tweenTime).OnComplete(() => graphicsRenderer.DOColor(previousColor, tweenTime));
			}
			if (amount > 0)
			{
				float tweenTime = 0.1f;
				Color previousColor = graphicsRenderer.color;
				graphicsRenderer.DOColor(onHealColor, tweenTime).OnComplete(() => graphicsRenderer.DOColor(previousColor, tweenTime));
			}
		}
		private void OnDestroy()
		{
			// If the scene is currently not loaded then lets back out instead of trying to create new stuff
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