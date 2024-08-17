using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OceanAnomaly.Attributes;

public class MonsterMovement : MonoBehaviour
{
    public float timeFast = 0.05f;
    public float timeSlow = 0.01f;
    
    public float tailLeftX = -15f;
    public float tailMidX = -1f;
    public float tailRightX = 15f;
    
    public float tailMinY = 0f;
    public float tailMaxY = 30f;
    
    public float to = -15f;
    public float fro = 15f;
    
    public float waitCount;
    public float waitDuration = 2.5f;
    
    public Transform target;
    
    static float t = 0.0f;
    
    public float largeAmount = 10f;
    public float medAmount = 1f;
    public float smallAmount = 0.5f;
    
    float headDirectionPrevious;
    
    Vector3 tailLeft;
    Vector3 tailMiddle;
    Vector3 tailRight;
    
    Vector3 swayTo;
    Vector3 swayFro;
    
    void Start ()
    {
        tailLeft = new Vector3(tailLeftX, tailMaxY, 0f);
        tailMiddle = new Vector3(tailMidX, tailMinY, 0f);
        tailRight = new Vector3(tailRightX, tailMaxY, 0f);
        
        swayTo = new Vector3(to, 0f, 0f);
        swayFro = new Vector3(fro, 0f, 0f);
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
            
            waitCount = 0f;
        }
        //Otherwise, if the direction of the head is equal to its previous direction, animate the IK target between its current position and its middle position
        else if (headDirection == headDirectionPrevious)
        {
            Debug.Log("2");
            
            waitCount += Time.deltaTime;
            
            if (waitCount < waitDuration)
                target.localPosition = Vector3.Lerp(targetPos, tailMiddle, timeSlow);
            else
            {
                // animate the position of the game object...
                target.localPosition = Vector3.Lerp(swayTo, swayFro, t);
                
                // .. and increase the t interpolater
                t += smallAmount * Time.deltaTime;
                
                // now check if the interpolator has reached 1.0
                // and swap maximum and minimum so game object moves
                // in the opposite direction.
                if (t > 1.0f)
                {
                    Vector3 temp = swayFro;
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
            
            waitCount = 0f;
        }
        
        //Record the the previous direction of the head
        headDirectionPrevious = gameObject.transform.eulerAngles.z;
    }
}
