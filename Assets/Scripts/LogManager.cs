using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    public Transform logContent; 
    public GameObject logTextPrefab;
    public ScrollRect scrollRect;
    public int maxLogCount = 10;

    public AnimationCurve scrollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 

    private Queue<GameObject> logEntries = new Queue<GameObject>();



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

    public void AddLog(string message)
    {
        GameObject entry = Instantiate(logTextPrefab, logContent);
        entry.GetComponent<TMP_Text>().text = message;

        logEntries.Enqueue(entry);

        if (logEntries.Count > maxLogCount)
        {
            Destroy(logEntries.Dequeue());
        }

        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null; 

        float duration = 0.35f;
        float current = 0f;
        float start = scrollRect.verticalNormalizedPosition;
        float end = 0f;

        while (current < duration)
        {
            current += Time.deltaTime;
            float t = Mathf.Clamp01(current / duration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, end, scrollCurve.Evaluate(t));
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = end;
    }
}
