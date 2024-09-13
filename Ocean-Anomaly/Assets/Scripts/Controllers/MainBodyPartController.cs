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
                PolygonPath limbShape = new PolygonPath();
				// Create the points for the tentacle limb specifically
				Vector3[] tentacleLinePoints = OrderTentacleLimbLinePoints(limbController as TentacleLimbSpawner);
                Vector2[] tentacleShapePoints = OrderTentacleLimbShapePoints(limbController as TentacleLimbSpawner);
				// Add it to the limbPolyline
				limbLine.AddPoints(tentacleLinePoints);
				limbShape.AddPoints(tentacleShapePoints);
				// Draw the PolyLine
				DrawPolyLine(limbLine);
                DrawPolygon(limbShape);
			}
		}
		private Vector3[] OrderTentacleLimbLinePoints(TentacleLimbSpawner tentacleLimb)
		{
			// For each of our limbs, lets add it to the polyLine
			List<Limb> limbs = tentacleLimb.GetLimbs();
			// Make a list of the points in the order we need to draw them in
			Vector3[] limbLinePoints = new Vector3[limbs.Count * 2 + 1];
			// Add last offset point in the middle of the limbPoint list
			limbLinePoints[limbs.Count] = limbs[limbs.Count - 1].EndPoint.position;
			// Iterate through the list and populate the points we need
			for (int i = 0; i < limbs.Count; i++)
			{
				limbLinePoints[i] = limbs[i].LeftPoint.position;
				limbLinePoints[(limbLinePoints.Length - 1) - i] = limbs[i].RightPoint.position;
			}
			return limbLinePoints;
		}
		private Vector2[] OrderTentacleLimbShapePoints(TentacleLimbSpawner tentacleLimb)
		{
			// For each of our limbs, lets add it to the polyLine
			List<Limb> limbs = tentacleLimb.GetLimbs();
			// Make a list of the points in the order we need to draw them in
			Vector2[] limbShapePoints = new Vector2[limbs.Count * 2 + 1];
			// Add last offset point in the middle of the limbPoint list
			limbShapePoints[limbs.Count] = limbs[limbs.Count - 1].EndPoint.position;
			// Iterate through the list and populate the points we need
			for (int i = 0; i < limbs.Count; i++)
			{
				limbShapePoints[i].x = limbs[i].LeftPoint.position.x;
                limbShapePoints[i].y = limbs[i].LeftPoint.position.y;
				limbShapePoints[(limbShapePoints.Length - 1) - i].x = limbs[i].RightPoint.position.x;
                limbShapePoints[(limbShapePoints.Length - 1) - i].y = limbs[i].RightPoint.position.y;
			}
			return limbShapePoints;
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
		private void DrawPolygon(PolygonPath pathToDraw)
		{
			// Null check for camera because we need it to draw
			if (mainCamera == null)
			{
				return;
			}
			// Shapes API suggested drawing of a Polyline
			using (Draw.Command(mainCamera))
			{
				Draw.Polygon(pathToDraw, Color.red);
			}
		}
	}
}
