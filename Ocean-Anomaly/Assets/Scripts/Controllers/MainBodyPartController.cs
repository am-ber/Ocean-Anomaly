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
				DrawLineInLimb(PopulateDrawLines(limbController as TentacleLimbSpawner));
			}
		}
		private PolylinePath PopulateDrawLines(TentacleLimbSpawner tentacleLimb)
		{
			PolylinePath limbLine = new PolylinePath();
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
			// Add all the points we got, hopefully in order
			limbLine.AddPoints(limbPoints);
			return limbLine;
		}
		private void DrawLineInLimb(PolylinePath pathToDraw)
		{
			using (Draw.Command(mainCamera))
			{
				Draw.Polyline(pathToDraw, true, 0.5f, Color.red);
			}
		}
	}
}
