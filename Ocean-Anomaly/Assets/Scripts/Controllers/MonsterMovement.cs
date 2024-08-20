using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OceanAnomaly.Attributes;
using Unity.Mathematics;
using OceanAnomaly.Tools;

public class MonsterMovement : MonoBehaviour
{
    [Header("Time Properties")]
    [SerializeField]
    private float timeSlow = 0.01f;
    [SerializeField]
    private float timeFast = 0.05f;
    [SerializeField]
    [ReadOnly]
    private float waitCount;
    [SerializeField]
    private float waitDuration = 2.5f;
    
    [Header("Movement Properties")]
    public bool isSwaying;
    [ReadOnly]
    public float tiltDirection;
    [SerializeField]
    private float headAnglePrevious;

    void Update ()
    {
        AnimateTail();
    }
    
    void AnimateTail()
    {
        float headAngle = 0f;
        
        headAngle = gameObject.transform.eulerAngles.z;
        
        if (headAngle > headAnglePrevious)
        {
            tiltDirection = Mathf.Lerp(tiltDirection, -1f, timeFast);
        
            isSwaying = false;
            waitCount = 0f;
        }
        else if (headAngle == headAnglePrevious)
        {
            waitCount += Time.deltaTime;

            if (waitCount < waitDuration)
            {
                tiltDirection = Mathf.Lerp(tiltDirection, 0f, timeSlow);
                
                isSwaying = false;
            }
            else
                isSwaying = true;
        }
        else if (headAngle < headAnglePrevious)
        {
            tiltDirection = Mathf.Lerp(tiltDirection, 1f, timeFast);
            
            isSwaying = false;
            waitCount = 0f;
        }
            
        headAnglePrevious = transform.eulerAngles.z;
    }
}
