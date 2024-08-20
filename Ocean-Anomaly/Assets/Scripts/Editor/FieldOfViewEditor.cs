using UnityEngine;
using UnityEditor;
using OceanAnomaly.Attributes;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI() {
		FieldOfView fow = (FieldOfView)target;
		Vector3 viewAngleA = fow.directionFromAngle(-fow.viewAngle / 2, false);
		Vector3 viewAngleB = fow.directionFromAngle(fow.viewAngle / 2, false);

		Handles.color = Color.white;
		Handles.DrawWireArc(fow.transform.position, Vector3.forward, viewAngleB, fow.viewAngle, fow.viewRadius);
		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
		Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fow.visibleTargets) {
			Handles.DrawLine(fow.transform.position, visibleTarget.position);
		}
	}
}
