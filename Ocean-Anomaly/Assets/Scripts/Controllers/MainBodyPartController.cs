using OceanAnomaly.Components;
using OceanAnomaly.Tools;
using Shapes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OceanAnomaly.Controllers
{
	public class MainBodyPartController : MonoBehaviour
	{
		[SerializeField]
		private List<LimbController> limbControllers;
		[SerializeField]
		private Camera mainCamera;
		private void Start()
		{
			// Grab all children with the LimbController
			limbControllers = new List<LimbController>(GetComponentsInChildren<LimbController>());
		}
		private void Update()
		{
			foreach (LimbController limbController in limbControllers)
			{
				PolylinePath limbLine = new PolylinePath();
				// Create the points for the tentacle limb specifically
				Vector3[] tentaclePoints = OrderTentacleLimbPoints(limbController as TentacleLimbSpawner);
				// Add it to the limbPolyline
				limbLine.AddPoints(tentaclePoints);
				// Draw the PolyLine
				DrawPolyLine(limbLine);
			}
		}
		private Vector3[] OrderTentacleLimbPoints(TentacleLimbSpawner tentacleLimb)
		{
			// For each of our limbs, lets add it to the polyLine
			List<Limb> limbs = tentacleLimb.GetLimbs();
			// Make a list of the points in the order we need to draw them in
			Vector3[] limbPoints = new Vector3[limbs.Count * 2 + 1];
			// Add last offset point in the middle of the limbPoint list
			limbPoints[limbs.Count] = limbs[limbs.Count - 1].EndPoint.position;
			// Iterate through the list and populate the points we need
			for (int i = 0; i < limbs.Count; i++)
			{
				limbPoints[i] = limbs[i].LeftPoint.position;
				limbPoints[(limbPoints.Length - 1) - i] = limbs[i].RightPoint.position;
			}
			return limbPoints;
		}
		private void DrawPolyLine(PolylinePath pathToDraw)
		{
			// Null check for camera because we need it to draw
			if (mainCamera == null)
			{
				return;
			}
			// Shapes API suggested drawing of a Polyline
			using (Draw.Command(mainCamera))
			{
				Draw.Polyline(pathToDraw, true, 0.5f, Color.red);
			}
		}
	}
}
