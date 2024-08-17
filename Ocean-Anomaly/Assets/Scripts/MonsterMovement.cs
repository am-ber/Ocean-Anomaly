using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float timeFast = 0.05f;
    public float timeSlow = 0.01f;
    
    public float swayTo = -15f;
    public float swayFro = 15f;
    
    public float waitCount;
    public float waitDuration;
    
    public Transform target;
    
    static float t = 0.0f;
    
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
        //Curl the tail in response to the head's rotation
        CurlTail();
    }
    
    void CurlTail ()
    {
        //Record the direction of the head
        float headDirection = 0f;
        
        headDirection = gameObject.transform.eulerAngles.z;
        
        //Record the position of the IK target
        Vector3 targetPos = new Vector3(target.localPosition.x, target.localPosition.y, target.localPosition.z);
        
        //If the direction of the head is greater than its previous direction, aniamte the IK target between its current position and its left position
        if (headDirection > headDirectionPrevious)
        {
            Debug.Log("1");
            
            target.localPosition = Vector3.Lerp(targetPos, tailLeft, timeFast);
        }
        //Otherwise, if the direction of the head is equal to its previous direction, animate the IK target between its current position and its middle position
        else if (headDirection == headDirectionPrevious)
        {
            Debug.Log("2");
            
            for ()
            target.localPosition = Vector3.Lerp(targetPos, tailMiddle, timeSlow);
            
            currentWaitTime += Time.deltaTime;
            
            if (currentWaitTime >= waitDuration)
            {
                // animate the position of the game object...
                target.localPosition = new Vector3(Mathf.Lerp(swayTo, swayFro, t), 5f, 0);
                
                // .. and increase the t interpolater
                t += 0.5f * Time.deltaTime;
                
                // now check if the interpolator has reached 1.0
                // and swap maximum and minimum so game object moves
                // in the opposite direction.
                if (t > 1.0f)
                {
                    float temp = swayFro;
                    swayFro = swayTo;
                    swayTo = temp;
                    t = 0.0f;
                }
            }
        }
        //Otherwise, if the direction of the head is less than its previous direction, animate the IK target between its current position and its right position
        else if (headDirection < headDirectionPrevious)
        {
            Debug.Log("3");
            
            target.localPosition = Vector3.Lerp(targetPos, tailRight, timeFast);
        }
        
        //Record the the previous direction of the head
        headDirectionPrevious = gameObject.transform.eulerAngles.z;
    }
}
