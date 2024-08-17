using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    
    public float timeFactor = 0.5f;
    
    static float t = 0.0f;
    
    public Transform target;
    
    float headDirectionPrevious;
    
    Vector3 tailLeft;
    Vector3 tailMiddle;
    Vector3 tailRight;
    
    void Start ()
    {
        tailLeft = new Vector3(-15f, 30f, 0f);
        tailMiddle = new Vector3(-1f, 0f, 0f);
        tailRight = new Vector3(15f, 30f, 0f);
    }

    void Update ()
    {
        //Move the tail in response to the rotation of the head
        MoveTail();
    }
    
    void MoveTail ()
    {
        //Record the direction of the head
        float headDirection = 0f;
        
        headDirection = gameObject.transform.rotation.z;
        
        //Record the position of the IK target
        Vector3 targetPos = new Vector3(target.localPosition.x, target.localPosition.y, target.localPosition.z);
        
        //If the direction of the head is greater than its previous direction, aniamte the IK target between its current position and its left position
        if (headDirection > headDirectionPrevious)
        {
            Debug.Log("1");
            
            target.position = Vector3.Lerp(targetPos, tailLeft, timeFactor);
        }
        //Otherwise, if the direction of the head is equal to its previous direction, animate the IK target between its current position and its middle position
        else if (headDirection == headDirectionPrevious)
        {
            Debug.Log("2");
            
            target.position = Vector3.Lerp(targetPos, tailMiddle, timeFactor);
        }
        //Otherwise, if the direction of the head is less than its previous direction, animate the IK target between its current position and its right position
        else if (headDirection < headDirectionPrevious)
        {
            Debug.Log("3");
            
            target.position = Vector3.Lerp(targetPos, tailRight, timeFactor);
        }
        
        //Record the the previous direction of the head
        headDirectionPrevious = gameObject.transform.rotation.z;
    }
}
