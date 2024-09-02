using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class IndicatorController : MonoBehaviour
{
	[Header("Attack Indicator Variables")]
	[SerializeField]
	private List<SpriteRenderer> attackIndicators;
	[SerializeField]
	private float attackIndicatorFadeAmount = 0.03f;
	[ReadOnly]
	[SerializeField]
	private float attackAlpha = 1f;
	[Header("Aiming Indicator Variables")]
	[SerializeField]
	private List<Triangle> aimingIndicators;
	[SerializeField]
	private float aimingIndicatorFadeAmount = 0.01f;
	[SerializeField]
	private float aimingIndicatorTimeActive = 1f;
	[ReadOnly]
	[SerializeField]
	private float timeTillAimingFade = 0f;
	[ReadOnly]
	[SerializeField]
	private float aimingAlpha = 1f;
	[ReadOnly]
	[SerializeField]
	private bool recentlyAimed = false;
	private void Update()
	{
		if (attackAlpha > 0f)
		{
			attackAlpha -= attackIndicatorFadeAmount;
			foreach (SpriteRenderer sprites in attackIndicators)
			{
				Color currentColor = sprites.color;
				currentColor.a = attackAlpha;
				sprites.color = currentColor;
			}
		}
		if (recentlyAimed)
		{
			timeTillAimingFade += Time.deltaTime;
			if (timeTillAimingFade > aimingIndicatorTimeActive)
			{
				aimingAlpha -= aimingIndicatorFadeAmount;
				foreach (Triangle triangle in aimingIndicators)
				{
					Color currentColor = triangle.Color;
					currentColor.a = aimingAlpha;
					triangle.Color = currentColor;
				}
				if (aimingAlpha <= 0f)
				{
					recentlyAimed = false;
				}
			}
		}
	}
	public void Fired()
	{
		attackAlpha = 1f;
	}
	public void Aiming()
	{
		aimingAlpha = 1f;
		timeTillAimingFade = 0f;
		recentlyAimed = true;
	}
}
