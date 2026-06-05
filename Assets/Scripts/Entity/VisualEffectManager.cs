using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualEffectManager : MonoBehaviour
{
   

    GameObject crackingOverlay;
    Image crackingSpriteOverlay;
    Entity entity;


    public ParticleSystem healingParticleSystem;






    void Start()
    {
        crackingOverlay = FindChildWithTag(transform, "CrackingOverlay").gameObject;
        entity = GetComponent<Entity>();

        
        crackingSpriteOverlay = crackingOverlay.GetComponentInChildren<Image>(true); //find the crack overlay image in the children of the entity

        crackingSpriteOverlay.material = MaterialPallet.Instance.getColoredMaterial(
            MaterialPallet.Instance.getOriginColor(entity.entityOrigin), 
            MaterialPallet.Instance.crackOverlayMaterial
        );
        crackingSpriteOverlay.material.SetFloat("_Health", 1 -((float)entity.getHP() / (float)entity.getMaxHP()) );
        crackingSpriteOverlay.material.SetVector("_Random", new Vector2(Random.value, Random.value)); //randomize the crack overlay offset
    }


    public void PlayParticleEffect(ParticleSystem particleSystem)
    {
        
        particleSystem.Play();
        
    }


    private Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
                return child;

            Transform result = FindChildWithTag(child, tag);
            if (result != null)
                return result;
        }

        return null;
    }
}
