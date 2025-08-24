using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_Manager : MonoBehaviour
{

    public GameObject triggerObj;
    public GameObject goalObj;
    public GameObject trackObj;
    public Transform trackStart;
    public Transform trackEnd;

    [Space(10)]
    [Range(0f, 1f)] public float t;
    public float speed = 1f;


    private RectTransform trigger;
    private RectTransform goal;
    private RectTransform track;

    private bool isGoing = true;


    // Start is called before the first frame update
    void Start()
    {

        track = trackObj.GetComponent<RectTransform>();
        goal = goalObj.GetComponent<RectTransform>();
        trigger = triggerObj.GetComponent<RectTransform>();


     

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.L)) {
            if (goal.GetComponent<BoxCollider2D>().bounds.Contains(trigger.transform.position))
            {
                Debug.Log("SUCCESS");
            }
            else
            {
                Debug.Log("MISS");
            }
        }

        //gets distance and direction
            if (isGoing)
            {
                t += speed * Time.deltaTime;
                if (t >= 1) { isGoing = false; }
            }
            else
            {
                t -= speed * Time.deltaTime;
                if (t <= 0) { isGoing = true; }
            }
        




        // Move trigger between start and end
        trigger.position = Vector3.Lerp(trackStart.position, trackEnd.position, t);

    }
    
        
}
