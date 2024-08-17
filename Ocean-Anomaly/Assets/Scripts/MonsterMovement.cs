using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float leftX = -15.0F;
    public float middleX = -1f;
    public float rightX =  15.0F;
    
    public float minimumY = 0f;
    public float maximumY = 30f;
    
    public float timeFactor = 0.5f;
    
    static float t = 0.0f;
    
    public Transform target;
    
    float headDirectionPrevious;
    
    Vector3 targetPosPrevious;

    void Update ()
    {
        //Move the tail in response to the rotation of the head
        MoveTail();
    }
    
    void MoveTail ()
    {
        //Record the direction of the head
        float headDirection = gameObject.transform.rotation.z;
        
        //Record the position of the IK target
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, target.position.z);
        
        //If the direction of the head is greater than its previous direction, aniamte the IK target between its current position and its right position
        if (headDirection > headDirectionPrevious)
        {
            target.position = new Vector3(Mathf.Lerp(targetPos.x, rightX, t), Mathf.Lerp(targetPos.y, maximumY, t), 0);
            
            Debug.Log("1");
        }
        //Otherwise, if the direction of the head is equal to its previous direction, animate the IK target between its current position and its middle position
        else if (headDirection == headDirectionPrevious)
        {
            target.position = new Vector3(Mathf.Lerp(targetPos.x, middleX, t), Mathf.Lerp(targetPos.y, minimumY, t), 0);
            
            Debug.Log("2");
        }
        //Otherwise, if the direction of the head is less than its previous direction, animate the IK target between its current position and its left position
        else if (headDirection < headDirectionPrevious)
        {
            target.position = new Vector3(Mathf.Lerp(targetPos.x, leftX, t), Mathf.Lerp(targetPos.y, maximumY, t), 0);
            
            Debug.Log("3");
        }
        
        t = timeFactor * Time.deltaTime;
        
        //Record the the previous direction of the head
        headDirectionPrevious = gameObject.transform.rotation.z;
        
        //Create a Vector 3 to store the position of the IK Target
        targetPosPrevious = new Vector3(target.position.x, target.position.y, target.position.z);
    }
}
