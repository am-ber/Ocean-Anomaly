using OceanAnomaly.Attributes;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using OceanAnomaly.Tools;

namespace OceanAnomaly.Components
{
    public class SpriteEffects : MonoBehaviour
    {
		[SerializeField]
		private float fadeSpeed = 1.0f;
		[Range(0f, 1f)]
		[SerializeField]
		private float fadeEndAlpha = 0f;
		[ColorUsage(true, true)]
		[SerializeField]
		private Color flashColor = Color.red;
		[SerializeField]
		private float flashSpeed = 0.1f;
		[SerializeField]
		private SpriteRenderer graphicsRenderer;
		[SerializeField]
		private GameObject spawnableGFXPrefab;
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
			if (graphicsRenderer == null)
			{
				graphicsRenderer = gameObject.RecursiveFindComponentLocal<SpriteRenderer>();
			}
		}
		public void FadeSprite()
		{
			graphicsRenderer.DOFade(fadeEndAlpha, fadeSpeed);
		}
		public void FlashSpriteColor()
		{
			Color previousColor = graphicsRenderer.color;
			graphicsRenderer.DOColor(flashColor, flashSpeed).OnComplete(() => graphicsRenderer.DOColor(previousColor, flashSpeed));
		}
		public void SpawnGFX()
		{
			// If the scene is currently not loaded then lets back out instead of trying to create new stuff
			if (!gameObject.scene.isLoaded)
			{
				return;
			}
			if (spawnableGFXPrefab != null)
			{
				Instantiate(spawnableGFXPrefab, transform.position, transform.rotation);
			}
		}
	}
}