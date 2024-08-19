using Dreamteck.Splines;
using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System;
using System.Collections;
using UnityEngine;

public class EnemyFieldManager : MonoBehaviour
{
	[SerializeField]
	private float pointGenerationRadius = 100f;
	[SerializeField]
	private float pointGenerationRate = 1.0f;
	[ReadOnly]
	[SerializeField]
	private Vector3 pointPosition = Vector3.zero;
	[SerializeField]
	private SplineComputer splineComputer;

	private void Start()
	{
		if (splineComputer == null)
		{
			splineComputer = GetComponent<SplineComputer>();
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, pointGenerationRadius);
	}
	private void Update()
	{

	}
}
