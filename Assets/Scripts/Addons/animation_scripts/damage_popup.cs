using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class damage_popup : MonoBehaviour
{
    public TextMeshProUGUI text;
    [Header("Motion")]
    public float minAngle = 60f;
    public float maxAngle = 120f;
    public float startSpeed = 150f;
    public float gravity = -300f;

    [Header("Lifetime")]
    public float duration = 1f;

    private Vector3 velocity;
    private float timer;

    void Start()
    {
        // Pick random upward angle
        float angle = Random.Range(minAngle, maxAngle);

        float radians = angle * Mathf.Deg2Rad;

        // Direction from angle
        Vector3 dir = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians),
            0f
        );

        velocity = dir * startSpeed;
    }

    void Update()
    {
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move popup
        transform.position += velocity * Time.deltaTime;

        timer += Time.deltaTime;

        if (timer >= duration)
            Destroy(gameObject);
    }
}
