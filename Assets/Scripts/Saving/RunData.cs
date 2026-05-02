using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is meant to hold all the data that needs to be saved across the run 
/// </summary> <summary>
/// 
/// </summary>
public class RunData : MonoBehaviour
{
    public static RunData Instance { get; private set; }

    [SerializeField] public DungeonMemory dungeonMemory = new DungeonMemory();

    private void Awake()
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
