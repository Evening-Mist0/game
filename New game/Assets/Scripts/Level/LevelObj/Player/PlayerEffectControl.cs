using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectControl : MonoBehaviour
{
    private Animator animator;
    
    public void PlayAtk()
    {
        animator.SetTrigger("Atk");
    }
}
