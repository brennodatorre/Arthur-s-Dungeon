using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPallet : MonoBehaviour
{
    public static MaterialPallet Instance;

    [Header("Standard Colors")]
    public Color purple;
    public Color blue;
    public Color red;
    public Color green;
    public Color Orange;
    public Color pink;
    public Color white;
    public Color yellow;
    
    [Space(10)]
    [Header("Dissolve Materials")]
    public Material dissolveMaterial;


    [Space(10)]
    [Header("Sprite Outline Materials")]
    public Material outlineSpriteMaterial;


    public Color getEntityOriginColor(Entity _entity)
    {
        switch (_entity.entityOrigin)
        {
            case Entity.EntityOrigin.FLAME:
                return purple;
            case Entity.EntityOrigin.ARTHUR:
                return blue;
            case Entity.EntityOrigin.ROSES:
                return red;
            case Entity.EntityOrigin.HEX:
                return green;
            case Entity.EntityOrigin.SYSTEM:
                return Orange;
            case Entity.EntityOrigin.LANDREAS:
                return pink;
            case Entity.EntityOrigin.UNKNOWN:
                return white;
            case Entity.EntityOrigin.SURVIVOR:
                return yellow;
            default:
                return Color.black;
        }
    }
    
    public Color getItemOriginColor(Item _item)
    {
        switch (_item.itemOrigin)
        { 
            case Entity.EntityOrigin.FLAME:
                return purple;
            case Entity.EntityOrigin.ARTHUR:
                return blue;
            case Entity.EntityOrigin.ROSES:
                return red;
            case Entity.EntityOrigin.HEX:
                return green;
            case Entity.EntityOrigin.SYSTEM:
                return Orange;
            case Entity.EntityOrigin.LANDREAS:
                return pink;
            case Entity.EntityOrigin.UNKNOWN:
                return white;
            case Entity.EntityOrigin.SURVIVOR:
                return yellow;
            default:
                return Color.black;
        }    
    }

    // creates an instance of a material and sets its color by one of the standar colors
    public Material getColoredMaterial(Color _standartColor, Material _materail)
    {
        Material newMat = new Material(_materail);
        newMat.color = _standartColor;
        return newMat;
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
    }
}
