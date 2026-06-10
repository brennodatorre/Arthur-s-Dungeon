using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Properties;

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
    [Header("Materials")]
    [Space(10)]
    public Material dissolveMaterial;
    [Space(10)]
    public Material outlineSpriteMaterial;
    [Space(10)]
    public Material crackMaterial;
    [Space(10)]
    public Material crackOverlayMaterial;


    public Color getOriginColor(Origin _origin)
    {
        switch (_origin)
        {
            case Origin.FLAME:
                return purple;
            case Origin.ARTHUR:
                return blue;
            case Origin.ROSES:
                return red;
            case Origin.HEX:
                return green;
            case Origin.SYSTEM:
                return Orange;
            case Origin.LANDREAS:
                return pink;
            case Origin.UNKNOWN:
                return white;
            case Origin.SURVIVOR:
                return yellow;
            default:
                return Color.black;
        }
    }
    
    

    /// <summary>
    /// creates an instance of a material and sets its color by one of the standar colors
    /// </summary>
    public Material getColoredMaterial(Color _standartColor, Material _materail)
    {
        Material newMat = new Material(_materail);
        newMat.SetColor("_Color", _standartColor);
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
