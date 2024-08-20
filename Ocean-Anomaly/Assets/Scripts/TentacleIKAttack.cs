using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleIKAttack : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField]
    private Transform target;
    
    void AttackPlayer()
    {
        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
    }
}
