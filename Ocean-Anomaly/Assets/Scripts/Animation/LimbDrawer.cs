using OceanAnomaly.Attributes;
using OceanAnomaly.Components;
using OceanAnomaly.Controllers;
using OceanAnomaly.Tools;
using Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OceanAnomaly.Animation
{
	public class LimbDrawer : MonoBehaviour
	{
		[SerializeField]
		private Camera mainCamera;
		[SerializeField]
		private LimbController limbController;
		public Sprite LimbStartGraphic;
		public Sprite LimbMiddleGraphic;
		public Sprite LimbEndGraphic;
		private void Start()
		{
			// Look for the limbController
			if (limbController == null)
			{
				limbController = gameObject.RecursiveFindComponentLocal<LimbController>();
			}
		}
		private void DrawShapesUpdate()
		{
			PolylinePath limbLine = new PolylinePath();
			PolygonPath limbShape = new PolygonPath();
			// Create the points for the tentacle limb specifically
			Vector3[] tentacleLinePoints = OrderTentacleLimbLinePoints(limbController as TentacleLimbSpawner);
			// Add it to the limbPolyline
			limbLine.AddPoints(tentacleLinePoints);
			limbShape.AddPoints(tentacleLinePoints.ToVector2Array());
			// Draw the PolyLine
			DrawPolyLine(limbLine);
			DrawPolygon(limbShape);
		}
		private Vector3[] OrderTentacleLimbLinePoints(TentacleLimbSpawner tentacleLimb)
		{
			// For each of our limbs, lets add it to the polyLine
			List<TentacleLimb> limbs = tentacleLimb.GetLimbs();
			// Make a list of the points in the order we need to draw them in
			Vector3[] limbLinePoints = new Vector3[limbs.Count * 2 + 1];
			// Add last offset point in the middle of the limbPoint list
			limbLinePoints[limbs.Count] = limbs[limbs.Count - 1].EndPoint.position;
			// Iterate through the list and populate the points we need
			for (int i = 0; i < limbs.Count; i++)
			{
				// Set the limbPoints
				limbLinePoints[i] = limbs[i].LeftPoint.position;
				limbLinePoints[(limbLinePoints.Length - 1) - i] = limbs[i].RightPoint.position;
			}
			return limbLinePoints;
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
				Draw.Polygon(pathToDraw, PolygonTriangulation.FastConvexOnly, Color.red);
			}
		}
	}
}
