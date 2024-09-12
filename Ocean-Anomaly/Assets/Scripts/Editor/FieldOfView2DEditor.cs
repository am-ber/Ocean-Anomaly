using OceanAnomaly.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView2D))]
public class FieldOfView2DEditor : Editor
{
	void OnSceneGUI()
	{
		FieldOfView2D fov2D = (FieldOfView2D)target;
		Vector3 viewAngleA = fov2D.DirectionFromAngle(-fov2D.viewAngle / 2);
		Vector3 viewAngleB = fov2D.DirectionFromAngle(fov2D.viewAngle / 2);

		Handles.color = Color.white;
		Handles.DrawWireArc(fov2D.viewTransform.position, Vector3.forward, viewAngleB, fov2D.viewAngle, fov2D.viewDistance);
		Handles.DrawLine(fov2D.viewTransform.position, fov2D.viewTransform.position + viewAngleA * fov2D.viewDistance);
		Handles.DrawLine(fov2D.viewTransform.position, fov2D.viewTransform.position + viewAngleB * fov2D.viewDistance);

		Handles.color = Color.red;
		foreach (Collider2D visibleTarget in fov2D.GetTargetsInField())
		{
			Handles.DrawLine(fov2D.viewTransform.position, visibleTarget.transform.position);
		}
	}
}
