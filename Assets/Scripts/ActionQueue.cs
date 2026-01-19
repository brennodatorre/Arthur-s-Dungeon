using System;
using System.Collections;
using System.Collections.Generic;
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
