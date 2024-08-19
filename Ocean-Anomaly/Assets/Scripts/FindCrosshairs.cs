using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCrosshairs : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField]
    private Transform target;
    
    [Header("Movement Properties")]
    [SerializeField]
    public float speed = 1.0f;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.up, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}