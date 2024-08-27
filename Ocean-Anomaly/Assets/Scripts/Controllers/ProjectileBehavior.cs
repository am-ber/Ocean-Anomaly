using OceanAnomaly;
using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using OceanAnomaly.Managers;
using UnityEngine;

public class ProjectileBehavior : BasicMoveBehavior
{
	public uint damage = 5;
	public string impactSoundName = string.Empty;
	public GameObject impactPrefab;
	[SerializeField]
	private float destroyEffectTime = 1f;
	[SerializeField]
	private float torqueDeviationRange = 10;
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Health colliderHealth = collision.gameObject.GetComponent<Health>();
		if (colliderHealth != null)
		{
			Debug.Log($"Found Health, Dealing: {damage} damage.");
			colliderHealth.ModifyHealth(-damage);
			if (impactPrefab != null)
			{
				Instantiate(impactPrefab, transform.position, Quaternion.Inverse(transform.rotation));
			}
		}
		AudioManager.Instance.Play(impactSoundName);
		Destroy(gameObject);
	}
	private void FixedUpdate()
	{
		FixedBasicMove();
	}
	private void OnDestroy()
	{
		GetComponent<Rigidbody2D>().AddTorque(Random.Range(-torqueDeviationRange, torqueDeviationRange));

	}
}
