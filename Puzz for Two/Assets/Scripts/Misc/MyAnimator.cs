using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class AnimatorEditorHelper : MonoBehaviour {
    Animator animComp;
    SpriteRenderer spriteComp;
    private void Start()
    {
        animComp = GetComponentInChildren<Animator>();
        spriteComp = GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        if (animComp.runtimeAnimatorController)
        {
            
        }
    }
}
