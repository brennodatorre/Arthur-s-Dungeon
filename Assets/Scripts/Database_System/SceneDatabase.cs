using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Scene Database")]
public class SceneDatabase : ScriptableObject
{
    public List<string> scenes;

    /// <summary>
    ///  Opens a random scene from the database
    /// </summary>
    /// 
    public string getRandom()
    {
        if (scenes.Count == 0) System.Diagnostics.Debug.WriteLine("No scenes in database");

        int index = Random.Range(0, scenes.Count);

        return scenes[index];
    }

   

        
    
}
