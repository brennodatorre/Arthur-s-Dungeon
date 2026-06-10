using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighLightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Material original;
    private Image img;
    private Material highLightMat;




    public Color colorMat;
    public bool isEffectActive = true;

    void Awake()
    {
        img = GetComponent<Image>();
        highLightMat = MaterialPallet.Instance.getColoredMaterial(colorMat, MaterialPallet.Instance.outlineSpriteMaterial);   
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isEffectActive) return; 
        original = img.material;
        img.material = highLightMat;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEffectActive) return; 
        img.material = original;
    }


    public void TurnOffEffect()
    {
        img.material = original;
        isEffectActive = false;

    }
}
