using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OceanAnomaly.Controllers;
public class MonsterAnimations : MonoBehaviour
{
    Animator anim;
    
    RuntimeAnimatorController runtimeAnimatorController;
    
    MonsterMovement movement;
    
    int tiltParamID;
    int swayParamID;

    // Start is called before the first frame update
    void Start()
    {
        tiltParamID = Animator.StringToHash("Tilt Direction");
        swayParamID = Animator.StringToHash("isSwaying");

		//If any of the needed components don't exist...
        anim = GetComponent<Animator>();
        movement = GetComponent<MonsterMovement>();
        
        if (anim == null || movement == null)
		{
			//...log an error and then remove this component
			Debug.LogError("A needed component is missing from the monster");
			Destroy(this);
		}
    }

    // Update is called once per frame
    void Update()
    {
		//Update the Animator with the appropriate values from the Player Movement script
        anim.SetFloat(tiltParamID, movement.tiltDirection);
        anim.SetBool(swayParamID, movement.isSwaying);
    }
}
