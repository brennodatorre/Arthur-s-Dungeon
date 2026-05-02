using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class DungeonMemory 
{
    
    [SerializeField]  List<string> triggeredEvents = new List<string>();

  
    public void Trigger(string eventtoTrigger)
    {
        
        if (!HasBeenTriggered(eventtoTrigger))
        {
            triggeredEvents.Add(eventtoTrigger);
        }
    }

    public bool HasBeenTriggered(string eventToCheck)
    {
        return triggeredEvents.Contains(eventToCheck);
    }

    public void Reset()
    {
        triggeredEvents.Clear();
    }

    public void PrintAllEvents()
{
    foreach (var e in triggeredEvents)
    {
        Debug.Log(e);
    }
}


}



public static class GameEvents
{
    public const string TalkedToOldRock = "Talked_To_Old_Rock";

    
}

