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
	[ReadOnly]
	[SerializeField]
	private int currentPoints = 0;
	[SerializeField]
	private SplineComputer splineComputer;
	[SerializeField]
	private GameObject fieldStart;
	private void Start()
	{
		Initialize();
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, pointGenerationRadius);
	}
	private void Initialize()
	{
		if (splineComputer == null)
		{
			splineComputer = GetComponent<SplineComputer>();
		}
		splineComputer.type = Spline.Type.CatmullRom;
		splineComputer.knotParametrization = 1f;
		splineComputer.is2D = true;

		GenerateNewPoints();
	}
	public SplineComputer GetSplineComputer()
	{
		return splineComputer;
	}
	/// <summary>
	/// Used to get the current starting position of the field spline.
	/// </summary>
	/// <returns></returns>
	public Vector3 GetFieldStartPosition()
	{
		if (fieldStart == null)
		{
			return Vector3.zero;
		}
		return fieldStart.transform.position;
	}
	public Transform GetFieldStart()
	{
		if (fieldStart == null)
		{
			return null;
		}
		return fieldStart.transform;
	}
	/// <summary>
	/// Used to generate new points randomly in a circle with the radius of <seealso cref="pointGenerationRadius"/>.
	/// </summary>
	public void GenerateNewPoints()
	{
		// 4 is the minimum required control points in the spline computer from the documentation
		currentPoints = UnityEngine.Random.Range(4, maxPoints + 1);
		SplinePoint[] splinePoints = new SplinePoint[currentPoints];
		for (int i = 0; i < splinePoints.Length; i++)
		{
			Vector3 pointPosition = GlobalTools.GetRandomPointInRadius2D(pointGenerationRadius, transform.position, pointGenerationRadiusMin);
			splinePoints[i] = new SplinePoint(pointPosition);
		}
		splineComputer.SetPoints(splinePoints);
		splineComputer.Close();

		if (fieldStart == null)
		{
			fieldStart = new GameObject("FieldStartObject");
			fieldStart.transform.parent = transform;
		}
		fieldStart.transform.position = splinePoints[0].position;
	}
}
