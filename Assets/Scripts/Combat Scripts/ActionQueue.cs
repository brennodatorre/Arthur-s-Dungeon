using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class ActionQueue : MonoBehaviour
{
    public Queue<Func<IEnumerator>> actionQueue = new Queue<Func<IEnumerator>>();
    public bool isRunning = false;
    public List<string> actionNames =  new List<string>();

    public int numActionsInQueue= 0;


    

    void Update()
    {
        numActionsInQueue = actionQueue.Count;
    }


    public void Enqueue(string name , Func<IEnumerator> action)
    {
        //Debug.Log(name);
        actionNames.Add(name); 
        actionQueue.Enqueue(action);

        if (!isRunning)
        {
            StartCoroutine(RunQueue());
        }
    }

    ///<summary>
    /// enqueue a new corountine based on a list of corountines, 
    /// they all start at once, but are added to the queue as one
    ///</summary>
    public void Enqueue(string name, List<Func<IEnumerator>> actions)
    {
        Enqueue(name, () =>runSimultaneousActions(actions) );
      
    }
    /// <summary>
    /// (Helper) runs a list of coroutines simultaneously
    /// </summary>
    private IEnumerator runSimultaneousActions(List<Func<IEnumerator>> actions)
    {
        foreach (var action in actions)
        {
            StartCoroutine(action());
            yield return null; // Wait for the next frame to start the next action
        }

    }

    private IEnumerator RunQueue()
    {
        isRunning = true;

        while (actionQueue.Count > 0)
        {
            Func<IEnumerator> action = actionQueue.Dequeue();
            //actionNames.RemoveAt(0); // Remove the action name from the list
            yield return StartCoroutine(action());
            
        }

        isRunning = false;
    }

    public bool IsRunning => isRunning;
}
