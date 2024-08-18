using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OceanAnomaly.Attributes;
using Unity.Mathematics;
using OceanAnomaly.Tools;

public class MonsterMovement : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField]
    private Transform tailSolver;
    
    [Header("Head Properties")]
    [SerializeField]
    [ReadOnly]
    private float headDirection;
    private float headDirectionPrevious;
    private float headIncline;
    
    [Header("Tail Properties")]
    [SerializeField]
    private float curlAngle = 165f;
    private float swayAngle = 20f;
    [ReadOnly]
    private float tailSign;
    
    [Header("Time Properties")]
    [SerializeField]
    private float waitDuration = 2.5f;
    private float timeFast = 0.05f;
    private float timeSlow = 0.01f;
    [ReadOnly]
    private float waitCount;
    private float t = 0.0f;
    
    [Header("Misc Properties")]
    [SerializeField]
    private float smallAmount = 0.5f;
    
    [Header("Movement Noise Settings")]
    [SerializeField]
    private float noiseIncrement = 0.01f;
    [SerializeField]
    private float noiseScale = 0.05f;
    [ReadOnly]
    [SerializeField]
    private float noiseValue = 0f;
	[ReadOnly]
	[SerializeField]
	private float mappedNoiseValue = 0f;
    
    void Update ()
    {
        //Curl the tail in response to the head's rotation
        AnimateTail();
    }
    
    void AnimateTail ()
    {
        //Get the sign of the head's rotation
        headIncline = Mathf.Sign(headDirection - headDirectionPrevious);
        
        //Reset the current direction of the head
        headDirection = transform.eulerAngles.z;
    
        if (headDirection != headDirectionPrevious)
        {
            Debug.Log("1");
            
            TailCurl();
        }
        //Otherwise, if the direction of the head is equal to its previous direction, animate the IK target between its current position and its middle position
        else if (headDirection == headDirectionPrevious)
        {
            Debug.Log("2");
            
            waitCount += Time.deltaTime;
            
            if (waitCount < waitDuration)
                TailMiddle();
            else
                TailSway();
        }
        
        //Store the current rotation of the head
        headDirectionPrevious = transform.eulerAngles.z;
    }
    
    void TailCurl ()
    {
        //Store the target rotation to curl the tail
        float tiltAroundZ = curlAngle * headIncline;

        //Set the target rotation to tilt around the z axis
        Quaternion target = Quaternion.Euler(transform.rotation.x, transform.rotation.y, tiltAroundZ);

        //Dampen towards the target rotation
        tailSolver.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * timeFast);
        
        //Reset the wait counter
        waitCount = 0f;
    }
    
    void TailMiddle ()
    {
        //Store the target rotation to return the tail to a neutral position
        float tiltAroundZ = 0;

        //Set the target rotation to tilt around the z axis
        Quaternion target = Quaternion.Euler(transform.rotation.x, transform.rotation.y, tiltAroundZ);

        //Dampen towards the target rotation
        tailSolver.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * timeSlow);
    }
    
    void TailSway ()
    {
        //Store the target rotation to sway the tail slightly
        float tiltAroundZ = swayAngle * tailSign;
        
        //Set the target rotation to tilt around the z axis
        Quaternion target = Quaternion.Euler(transform.rotation.x, transform.rotation.y, tiltAroundZ);
        
        //Dampen towards the target rotation
        tailSolver.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * t);
        
        // Multiply deltaTime by a scale because noise moves very fast above 1.0
        noiseValue = Mathf.PerlinNoise1D(waitCount * noiseScale);
        // Remap the noise value from 0:1 to -1:1
        mappedNoiseValue = GlobalTools.Map(noiseValue, 0, 1, -1, 1);
        
        // .. and increase the t interpolater
        t += (smallAmount + mappedNoiseValue) * Time.deltaTime;
        
        // now check if the interpolator has reached 1.0
        // and swap maximum and minimum so game object moves
        // in the opposite direction.
        if (t > 1.0f)
        {
            tailSign *= -1f;
            t = 0.0f;
        }
    }
/*
    public float timeFast = 0.05f;
    public float timeSlow = 0.01f;
    
    public float tailCurve = 165f
    
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
    
    public float t = 0.0f;
    
    public float largeAmount = 10f;
    public float medAmount = 1f;
    public float smallAmount = 0.5f;
    
    float headDirectionPrevious;
    
    Vector3 tailLeft;
    Vector3 tailMiddle;
    Vector3 tailRight;
    
    Vector3 swayTo;
    Vector3 swayFro;

    [Header("Movement Noise Settings")]
    [SerializeField]
    private float noiseIncrement = 0.01f;
    [SerializeField]
    private float noiseScale = 0.05f;
    [ReadOnly]
    [SerializeField]
    private float noiseValue = 0f;
	[ReadOnly]
	[SerializeField]
	private float mappedNoiseValue = 0f;
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
                target.localPosition = Vector3.Lerp(targetPos, swayTo, timeSlow);
            else
            {
                // animate the position of the game object...
                target.localPosition = Vector3.Lerp(swayTo, swayFro, t);

				// Multiply deltaTime by a scale because noise moves very fast above 1.0
				noiseValue = Mathf.PerlinNoise1D(waitCount * noiseScale);
                // Remap the noise value from 0:1 to -1:1
                mappedNoiseValue = GlobalTools.Map(noiseValue, 0, 1, -1, 1);

				// .. and increase the t interpolater
				t += (smallAmount + mappedNoiseValue) * Time.deltaTime;
                
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
*/

}
