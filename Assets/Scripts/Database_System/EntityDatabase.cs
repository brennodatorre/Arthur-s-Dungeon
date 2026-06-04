using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Entity Database")]
public class EntityDatabase : ScriptableObject
{
    public List<GameObject> entitiesPrefabs;

    

    public GameObject GetEntity(string entityID)
    {
        foreach (GameObject prefab in entitiesPrefabs)
        {
            Entity ent = prefab.GetComponentInChildren<Entity>();

            if (ent.entityID == entityID)
            {
                return Instantiate(prefab);
            }
        }

        Debug.LogWarning("Entity not found: " + entityID);
        return null;
    }

   
}
