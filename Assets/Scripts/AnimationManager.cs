using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AnimationManager : MonoBehaviour
{
    public GameObject animatorOBJ;
    private Animator animator;
    public Vector3 offset;

    void Start()
    {
        animator = animatorOBJ.GetComponent<Animator>();
    }



    public void doAnimation(Entity target){

        StartCoroutine(playSlashAnima(target));
        
    }

    private IEnumerator playSlashAnima(Entity target){

        animatorOBJ.GetComponent<UnityEngine.UI.Image>().enabled = true;
 
        animatorOBJ.transform.position = target.transform.position + offset;

        animator.SetTrigger("doSlash");

        yield return null; //waits for next turn

    
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float animationLength = clipInfo.Length > 0 ? clipInfo[0].clip.length : 0.5f;


        yield return new WaitForSeconds(animationLength);


        animatorOBJ.GetComponent<UnityEngine.UI.Image>().enabled = false;



    }

}
