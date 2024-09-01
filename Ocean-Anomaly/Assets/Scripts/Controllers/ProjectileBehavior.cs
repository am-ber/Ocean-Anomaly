using OceanAnomaly;
using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using OceanAnomaly.Managers;
using UnityEngine;

public class ProjectileBehavior : BasicMoveBehavior
{
	public uint damage = 5;
	public SoundScriptable impactSound;
	public GameObject impactPrefab;
	[SerializeField]
	private float torqueDeviationRange = 10;
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Health colliderHealth = collision.gameObject.GetComponent<Health>();
		if (colliderHealth != null)
		{
			colliderHealth.ModifyHealth(-damage);
			if (impactPrefab != null)
			{
				Instantiate(impactPrefab, transform.position, Quaternion.Inverse(transform.rotation));
			}
		}
		// Play sound code
		if (impactSound != null && GlobalManager.Instance != null)
		{
			impactSound.Play();
		}
		Destroy(gameObject);
	}
	private void FixedUpdate()
	{
		FixedBasicMove();
	}
}
