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
		[SerializeField]
		private MeshFilter meshFilter;
		[SerializeField]
		private Mesh mesh;
		[SerializeField]
		private Vector3 meshNormal = Vector3.back;
		[ReadOnly]
		[SerializeField]
		private Vector3[] meshVertices;
		[ReadOnly]
		[SerializeField]
		private Vector3[] normals;
		[SerializeField]
		private bool drawPolygon = false;
		[SerializeField]
		private bool drawPolyline = false;
		private void Start()
		{
			// Only if the meshFilter is set can we draw the mesh
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			// Setup the mesh
			mesh = new Mesh();
			meshFilter.mesh = mesh;
			// Look for the limbController
			if (limbController == null)
			{
				limbController = gameObject.RecursiveFindComponentLocal<LimbController>();
			}
		}
		private void Update()
		{
			PolylinePath limbLine = new PolylinePath();
			PolygonPath limbShape = new PolygonPath();
			// Create the points for the tentacle limb specifically
			Vector3[] tentacleLinePoints = OrderTentacleLimbLinePoints(limbController as TentacleLimbSpawner);
			meshVertices = tentacleLinePoints;
			// Add it to the limbPolyline
			limbLine.AddPoints(tentacleLinePoints);
			limbShape.AddPoints(tentacleLinePoints.ToVector2Array());
			// Draw the PolyLine
			if (drawPolyline) DrawPolyLine(limbLine);
			if (drawPolygon) DrawPolygon(limbShape);
			// Create Mesh
			if (mesh != null)
			{
				mesh.name = $"{limbController.name} Mesh";
				CreateMesh(tentacleLinePoints);
			}
		}
		public void SetVertices(Vector3[] vertices)
		{
			meshVertices = vertices;
		}
		private Vector3[] OrderTentacleLimbLinePoints(TentacleLimbSpawner tentacleLimb)
		{
			// For each of our limbs, lets add it to the polyLine
			List<Limb> limbs = tentacleLimb.GetLimbs();
			// Make a list of the points in the order we need to draw them in
			Vector3[] limbLinePoints = new Vector3[limbs.Count * 2 + 1];
			// Add last offset point in the middle of the limbPoint list
			limbLinePoints[limbs.Count] = limbs[limbs.Count - 1].EndPoint.position;
			// Create the normals for the mesh so we don't need to loop twice through the same list
			normals = new Vector3[limbLinePoints.Length];
			// Iterate through the list and populate the points we need
			for (int i = 0; i < limbs.Count; i++)
			{
				// Set the limbPoints
				limbLinePoints[i] = limbs[i].LeftPoint.position;
				limbLinePoints[(limbLinePoints.Length - 1) - i] = limbs[i].RightPoint.position;
				// Set the normals to the normal param given
				normals[i] = meshNormal;
			}
			return limbLinePoints;
		}
		private void CreateMesh(Vector3[] points)
		{
			// Safety check the points array
			if ((points == null) || (points.Length <= 0))
			{
				return;
			}
			// Do mesh stuff
			mesh.Clear();
			mesh.vertices = points;
			mesh.normals = normals;
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
