using OceanAnomaly;
using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using OceanAnomaly.Managers;
using UnityEngine;

public class ProjectileBehavior : BasicMoveBehavior
{
	public float damage = 5;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"Hit: {collision.collider.name}");
	}
	private void FixedUpdate()
	{
		FixedBasicMove();
	}
}
