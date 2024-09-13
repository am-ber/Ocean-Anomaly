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
	private float pointMinimumCloseness = 25f;
	[SerializeField]
	private GameObject enemyTargetInRadius;
	private void Awake()
	{
		if (enemyTargetInRadius == null)
		{
			enemyTargetInRadius = Instantiate(new GameObject("FieldStartObject"), transform);
		}
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, pointGenerationRadius);
		if (enemyTargetInRadius != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(enemyTargetInRadius.transform.position, 2);
		}
	}
	/// <summary>
	/// Used to get the current starting position of the field spline.
	/// </summary>
	/// <returns></returns>
	public Vector3 GetEnemyTargetPosition()
	{
		if (enemyTargetInRadius == null)
		{
			return Vector3.zero;
		}
		return enemyTargetInRadius.transform.position;
	}
	public Transform GetEnemyTargetTransform()
	{
		if (enemyTargetInRadius == null)
		{
			return null;
		}
		return enemyTargetInRadius.transform;
	}
	/// <summary>
	/// Used to change the target transform position randomly in a circle with the radius of <seealso cref="pointGenerationRadius"/>.
	/// </summary>
	public void ChangeTargetPosition()
	{
		Vector3 newPosition = enemyTargetInRadius.transform.position;
		// Keep looping until we have a distance that is a desired distance away from the mimimum closeness allowed
		do
		{
			newPosition = GlobalTools.GetRandomPointInRadius2D(pointGenerationRadius, transform.position, pointGenerationRadiusMin);
			// This while loop checks for the distance between the point we just generated and the current target position
			// to be less than the minimumCloseness BUT only if that is less than 90% of the total generation radius
		} while (Vector3.Distance(enemyTargetInRadius.transform.position, newPosition) <
		(pointMinimumCloseness < (pointGenerationRadius * 0.9f) ? pointMinimumCloseness : (pointGenerationRadius * 0.9f)));
		// Set the target position
		enemyTargetInRadius.transform.position = newPosition;
	}
}
