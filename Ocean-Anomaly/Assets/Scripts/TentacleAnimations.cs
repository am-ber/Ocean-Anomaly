using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleAnimations : MonoBehaviour
{
    [Header("Bool")]
    [SerializeField]
    private bool isAttacking;

    Animator anim;
    RuntimeAnimatorController runtimeAnimatorController;
    
    int attackParamID;
    
    void Start()
    {
        attackParamID = Animator.StringToHash("isAttacking");
        
        anim = GetComponent<Animator>();
        
        if (anim == null)
        {
            Debug.LogError("A needed component is missing from the monster");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool(attackParamID, isAttacking);
    }
}
