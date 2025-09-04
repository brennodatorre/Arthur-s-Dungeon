using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPallet : MonoBehaviour
{
    public static MaterialPallet Instance;

    [Header("Dissolve Materials")]
    public Material dissolvePurple;
    public Material dissolveBlue;
    public Material dissolveRed;
    public Material dissolveGreen;
    public Material dissolveOrange;
    public Material dissolvePink;
    public Material dissolveWhite;
    public Material dissolveYellow;



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
