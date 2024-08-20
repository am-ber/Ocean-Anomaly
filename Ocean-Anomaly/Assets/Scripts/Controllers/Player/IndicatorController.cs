using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour
{
	[Header("Attack Idicator Variables")]
	[SerializeField]
	private List<SpriteRenderer> attackIndicators;
	[SerializeField]
	private float attackIndicatorFadeAmount = 0.01f;
	[ReadOnly]
	[SerializeField]
	private bool attackIndicatorCoroutineRunning = false;
	private IEnumerator attackIndicatorCoroutine;
	private void Start()
	{
		FadeAttackIndicator();
	}
	public void ResetAttackIndicator()
	{
		if (!attackIndicatorCoroutineRunning)
		{
			StopCoroutine(attackIndicatorCoroutine);
		}
		attackIndicatorCoroutineRunning = false;
	}
	public void FadeAttackIndicator()
	{
		if (attackIndicatorCoroutineRunning)
		{
			return;
		}
		attackIndicatorCoroutine = FadeAttackIndicatorSprite();
		StartCoroutine(attackIndicatorCoroutine);
	}
	private IEnumerator FadeAttackIndicatorSprite()
	{
		attackIndicatorCoroutineRunning = true;
		float alpha = 1.0f;
		// This should slowly fade the indicator color
		while (alpha > 0)
		{
			alpha -= attackIndicatorFadeAmount;
			foreach (SpriteRenderer sprites in attackIndicators)
			{
				Color currentColor = sprites.color;
				currentColor.a = alpha;
				sprites.color = currentColor;
			}
			yield return new WaitForSeconds(attackIndicatorFadeAmount);
		}
		attackIndicatorCoroutineRunning = false;
	}
}
