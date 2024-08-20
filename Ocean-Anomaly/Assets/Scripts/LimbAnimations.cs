using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbAnimations : MonoBehaviour
{
    [Header("Bools")]
    [SerializeField]
    private bool isAttacking;
    
    [Header("Transforms")]
    [SerializeField]
    private Transform player;
    private Transform limbTarget

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
    
    public void AttackPlayer()
    {
        limbTarget.position = new Vector3(player.position.x, player.position.y, player.position.z);
    }
}
