using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using OceanAnomaly.Tools;

public class CreatureShapeDrawer : ImmediateModeShapeDrawer
{
	[Header("Transforms")]
	[TagSelector]
	[SerializeField]
	private string bodyPointTag;
	[SerializeField]
	Transform[] bodyPoints;

	[Header("Shape Properties")]
	[SerializeField]
	float lineThickness;

	[Header("Cameras")]
	[SerializeField]
	Camera cam;

	[SerializeField]
	PolylinePath creatureOutline;

	void Start()
	{
		bodyPoints = transform.FindChilderenByTag(bodyPointTag);
	}

	void Update()
	{
		ChartBody();
		DrawShapes(cam);
	}

	void ChartBody()
	{
		creatureOutline = new PolylinePath();
		for (int bP = 0; bP < bodyPoints.Length; bP++)
		{
			// Convert the position of the point below the parent
			Vector3 localPosition = bodyPoints[bP].localPosition;
			creatureOutline.AddPoint(localPosition.x, localPosition.y);
		}
	}

	public override void DrawShapes(Camera cam)
	{
		using (Draw.Command(cam))
		{
			// Set the matrix of drawing relative to the transform position of this transform
			Draw.Matrix = transform.localToWorldMatrix;
			Draw.Polyline(creatureOutline, closed: true, thickness: lineThickness, Color.red); // Drawing happens here
		}
	}
}