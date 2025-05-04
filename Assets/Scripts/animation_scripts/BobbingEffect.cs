using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingEffect : MonoBehaviour
{

    public bool active;
    [Space]
    public float amplitude = 5f; // height of the bob
    public float frequency = 3f;   // speed of the bob

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {   
        if (active) {
            float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.localPosition = startPos + new Vector3(0f, offsetY, 0f);
        }
    }
}
