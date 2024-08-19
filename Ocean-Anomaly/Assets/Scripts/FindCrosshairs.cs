using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCrosshairs : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField]
    private Transform crosshair;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(crosshair.position.x, crosshair.position.y, crosshair.position.z);
    }
}
