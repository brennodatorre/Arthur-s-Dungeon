using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    public ItemDatabase allItemsDatabase;
    public SkillDatabase allSkillsDatabase;
    public StatusEffectDatabase allStatusEffectsDatabase;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }


    }

    
}
