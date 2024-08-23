using OceanAnomaly.Attributes;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.Components
{
    public class SpriteEffects : MonoBehaviour
    {
		public float FadeAmountPerFrame = 0.75f;
		public Color flashColor = Color.red;
		public float flashSpeed = 1.5f;
		[SerializeField]
		private SpriteRenderer spriteRenderer;
		[ReadOnly]
		[SerializeField]
		private Color currentColor;
		private void Start()
		{
			if (spriteRenderer != null)
			{
				currentColor = spriteRenderer.color;
			}
		}
		public async void FadeSprite()
		{
			float alpha = 1.0f;
			while (alpha > 0)
			{
				currentColor.a = alpha;
				if (spriteRenderer != null)
					spriteRenderer.color = currentColor;
				alpha -= FadeAmountPerFrame * Time.deltaTime;
				await Task.Yield();
			}
		}
		public async void FlashSpriteColor()
		{
			float time = 0;
			while (time < 1)
			{
				if (spriteRenderer != null)
					spriteRenderer.color = Color.Lerp(currentColor, flashColor, time);
				time += (flashSpeed * Time.deltaTime);
				await Task.Yield();
			}
			time = 1;
			while (time > 0)
			{
				if (spriteRenderer != null)
					spriteRenderer.color = Color.Lerp(currentColor, flashColor, time);
				time -= (flashSpeed * Time.deltaTime);
				await Task.Yield();
			}
		}
	}
}