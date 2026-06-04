
using UnityEngine;

using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using System;
using System.Collections;

public class QTE_Manager : MonoBehaviour
{
    public static QTE_Manager Instance;

    public Sprite iconQ;
    public Sprite iconW;
    public Sprite iconE;
    public Sprite iconR;


    [Space(5)]
    public GameObject QTE;
    public GameObject trigger;
    public GameObject goal;
    public GameObject track;

    public Transform trackStart;
    public Transform trackEnd;

    public Color failColor;
    public Color sucessColor;



    [Space(10)]
    [Range(0f, 1f)] public float t;
    public float speed = 1f;
    public float critComboSpeedIncrease = 0.20f;

    [Space(3)]
    public bool suceededQTE;
    public bool qteIsRunning;




    private KeyCode keyCode;

    private Bounds triggerBounds;
    private Bounds goalBounds;

    private bool isGoing = true;


    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }


    }


    // Update is called once per frame
    public IEnumerator doQTE(int critCombo )
    {
        QTE.gameObject.SetActive(true);
        setQTE();
        qteIsRunning = true;

        float originalSpeed = speed;


        speed += critCombo * critComboSpeedIncrease; // increases the speed of the QTE based on the crit combo, making it more difficult to pull off higher combos, but rewarding player skill


        // deals with speed audio 
        AudioSource tempAudioSource = AudioManager.Instance.PlayQTESoundWithProgressivePitch
            (AudioManager.Instance.qteSpeedSound, () => t, speed, () => qteIsRunning)
        ;
        



        while (qteIsRunning)
        {



            //deals with getting collider status
            triggerBounds = trigger.GetComponent<BoxCollider2D>().bounds;
            goalBounds = goal.GetComponent<BoxCollider2D>().bounds;
            bool triggerFullyContained = (goalBounds.Contains(triggerBounds.min) && goalBounds.Contains(triggerBounds.max));

            
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) ||Input.GetKeyDown(KeyCode.E) ||Input.GetKeyDown(KeyCode.R))
            {
                if (triggerFullyContained && Input.GetKeyDown(keyCode))
                {
                    //stops and destroy the temporary audio source playing the speed sound
                    tempAudioSource.Stop();
                    Destroy(tempAudioSource);

                    yield return StartCoroutine(suceedQTE());
                }
                else
                {
                    //stops and destroy the temporary audio source playing the speed sound
                    tempAudioSource.Stop();
                    Destroy(tempAudioSource);
                    
                    yield return StartCoroutine(failedQTE());
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
                yield return StartCoroutine(failedQTE());
            }

            // Move trigger between start and end
            trigger.transform.position = Vector3.Lerp(trackStart.position, trackEnd.position, t);
            yield return null;
        }

        speed = originalSpeed; // resets speed for next qte

    }


    private IEnumerator failedQTE()
    {
        //resets for next qte
        isGoing = true;

        t = 0f;

        //Debug.Log("MISS");

        suceededQTE = false;
        AudioManager.Instance.PlaySound(AudioManager.Instance.qteFail);
        yield return StartCoroutine(endQTE(true));
        
    }

    private IEnumerator suceedQTE()
    {
        //resets for next qte
        isGoing = true;
        t = 0f;

        //Debug.Log("SUCCESS");

        suceededQTE = true;
        AudioManager.Instance.PlaySound(AudioManager.Instance.qteSucess);
        yield return StartCoroutine(endQTE(false));
        
    }

    private IEnumerator endQTE(bool failed)
    {
        Image triggerImage = trigger.GetComponent<Image>();
        Color originalTColor = triggerImage.color;

        goal.GetComponent<Image>().enabled = false;
        track.GetComponent<Image>().enabled = false;

        if (failed) {triggerImage.color = failColor; }
        else
        { 
            triggerImage.color = sucessColor;
        }
        

        yield return new WaitForSeconds(.5f);

        triggerImage.color = originalTColor;
        goal.GetComponent<Image>().enabled = true;
        track.GetComponent<Image>().enabled = true;


        QTE.gameObject.SetActive(false);
        qteIsRunning = false;
    }


    private void setQTE()
    {


        //adds random rotation to the QTE, 
        // while maintaining trigger and goal's rotation for visibility
        int rot = Random.Range(-170, 171);
        QTE.transform.rotation = Quaternion.Euler(0, 0, rot);
        trigger.transform.localRotation = Quaternion.Euler(0, 0, -rot);
        goal.transform.localRotation = Quaternion.Euler(0, 0, -rot);

        //sets a random key for the QTE
        int x = Random.Range(0, 4);
        switch (x)
        {
            case 0:
                keyCode = KeyCode.Q;
                trigger.GetComponent<Image>().sprite = iconQ;
                goal.GetComponent<Image>().sprite = iconQ;
                break;
            case 1:
                keyCode = KeyCode.W;
                trigger.GetComponent<Image>().sprite = iconW;
                goal.GetComponent<Image>().sprite = iconW;
                break;
            case 2:
                keyCode = KeyCode.E;
                trigger.GetComponent<Image>().sprite = iconE;
                goal.GetComponent<Image>().sprite = iconE;
                break;
            case 3:
                keyCode = KeyCode.R;
                trigger.GetComponent<Image>().sprite = iconR;
                goal.GetComponent<Image>().sprite = iconR;
                break;



        }
    }
    
        
}
