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
	private float pointGenerationRadiusMin = 0f;
	[SerializeField]
	private int maxPoints = 10;
	[SerializeField]
	private SplineComputer splineComputer;

	private void Start()
	{
		if (splineComputer == null)
		{
			splineComputer = GetComponent<SplineComputer>();
		}
		splineComputer.type = Spline.Type.Bezier;
		splineComputer.is2D = true;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, pointGenerationRadius);
	}
	public void SubscribeToSpline(SplineUser splineUser)
	{
		if (splineComputer == null)
		{
			return;
		}
		splineComputer.Subscribe(splineUser);
	}
	public Vector3 GetFieldStartPosition()
	{
		if (splineComputer == null)
		{
			return Vector3.zero;
		}
		return splineComputer.GetPoint(0).position;
	}
	public void UnsubscribeToSpline(SplineUser splineUser)
	{
		if (splineComputer == null)
		{
			return;
		}
		splineComputer.Unsubscribe(splineUser);
	}
	public void GenerateNewPoints()
	{
		// 4 is the minimum required control points in the spline computer from the documentation
		SplinePoint[] splinePoints = new SplinePoint[UnityEngine.Random.Range(4, maxPoints + 1)];
		for (int i = 0; i < splinePoints.Length; i++)
		{
			Vector3 pointPosition = GlobalTools.GetRandomPointInRadius2D(pointGenerationRadius, transform.position);
			splinePoints[i] = new SplinePoint(pointPosition);
		}
		splineComputer.SetPoints(splinePoints);
		splineComputer.Close();
	}
}
