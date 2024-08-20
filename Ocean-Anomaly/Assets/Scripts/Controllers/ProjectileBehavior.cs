using OceanAnomaly;
using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using OceanAnomaly.Managers;
using UnityEngine;

public class ProjectileBehavior : BasicMoveBehavior
{
	public float damage = 5;
	void Update()
	{
		rotateTo();
	}
	private void FixedUpdate()
	{
		FixedBasicMove();
	}
}
