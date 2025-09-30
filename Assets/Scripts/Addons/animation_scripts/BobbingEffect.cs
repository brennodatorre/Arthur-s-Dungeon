using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingEffect : MonoBehaviour
{
    
    public bool active = true;
    [Space]
    public float amplitude = 5f; // height of the bob
    public float frequency = 3f;   // speed of the bob
    public float startDelay = 0f;
    private float animationStartTime;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
        StartCoroutine(delayedStart(startDelay));
    }

    void Update()
    {   
        if (active) {
            float passedTime = Time.time - animationStartTime;
            float offsetY = Mathf.Sin(passedTime * frequency) * amplitude;
            transform.localPosition = startPos + new Vector3(0f, offsetY, 0f);
        }
    }


    private IEnumerator delayedStart(float delay) {
        if (active) {
            active = false;
            yield return new WaitForSeconds(delay);
            animationStartTime = Time.time;
            active = true;
        }

    }
}
