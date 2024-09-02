using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class CreatureShapeDrawer : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField]
    GameObject[] bodyPoints;
    
    [Header("Shape Properties")]
    [SerializeField]
    float lineThickness;
    Vector2[] bodyCoordinates;
    
    [Header("Cameras")]
    [SerializeField]
    Camera cam;
    
    PolylinePath creatureOutline;
    
    void Start()
    {
        bodyPoints = GameObject.FindGameObjectsWithTag("Body Point");
        
        creatureOutline = new PolylinePath();
    }
    
    void Update()
    {
        ChartBody();
        DrawShapes(cam);
    }
    
    void ChartBody()
    {
        for (int bP = 0; bP < bodyPoints.Length; bP++)
        {
            creatureOutline.AddPoint(bodyPoints[bP].transform.position.x, bodyPoints[bP].transform.position.y);
            
            Debug.Log(creatureOutline[1]);
        }
    }
    
    void DrawShapes(Camera cam)
    {
        Draw.Command(cam);
        Draw.Matrix = transform.localToWorldMatrix;
        Draw.Polyline(creatureOutline, closed:true, thickness:lineThickness, Color.red); // Drawing happens here
    }
}