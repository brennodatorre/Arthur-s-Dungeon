using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class damage_popup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatSpeed = 1f;
    public float duration = 1f;

    private float timer = 0f;

    public void Setup(float damage)
    {
        text.text = damage.ToString();
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
