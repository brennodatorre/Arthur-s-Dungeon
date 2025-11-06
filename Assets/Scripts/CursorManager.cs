using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{

    [HideInInspector] public static CursorManager Instance;


    [SerializeField] public GameObject customCursor;
    public Image ccImage;
    [SerializeField] private Canvas canvas;
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    [SerializeField] private Sprite base_cursor; // The default cursor sprite
    [SerializeField] private Sprite onClick_cursor; // The clicked cursor sprite
    [SerializeField] private Sprite blade_cursor; // The hovered cursor sprite
    [SerializeField] private Image paht_circle;

    private RoundManager roundManager;
    private MySceneManager sceneManager;


    public int base_cursorSize = 1; // Size of the cursor in pixels
    public int onClick_cursorSize = 1; // Size of the cursor in pixels
    public int blade_cursorSize = 1; // Size of the cursor in pixels


    [Space(10)]
    [Header("PAHT Settings")]
    public PressAndHoldTarget holdable;
    public PressAndHoldTarget holdableMEM;
    private Coroutine holdingCoroutine;
    private float holdTime = 0f;
    public float pahtDuration;
    public bool lookingForPAHT = false; 

    [Space(10)]
    public bool isDragging = false;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }


        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        setCursor(false, false);
        

    }

    void Start()
    {
        if (MySceneManager.Instance.sceneType == MySceneManager.SceneType.TEST) { return; }

        roundManager = RoundManager.Instance;
        sceneManager = MySceneManager.Instance;
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>(); 

    }


    void Update() {
        // blocks update if on 3D Space
        if (MySceneManager.Instance.sceneType == MySceneManager.SceneType.TEST) { return; }

        if (customCursor == null) { customCursor = GameObject.FindGameObjectWithTag("customCursor"); }
        if (sceneManager == null) { sceneManager = MySceneManager.Instance;}
        if (canvas == null ){ StartCoroutine(LoadingDelay()); return; }
        if (roundManager == null) {roundManager = RoundManager.Instance; }



        Vector3 mousePos = Input.mousePosition;

        if (sceneManager.sceneType == MySceneManager.SceneType.TEST) setCursor(false,true);
        
        if (sceneManager.sceneType != MySceneManager.SceneType.TEST)
        {
            // Convert the mouse position to canvas space
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), mousePos, canvas.worldCamera, out localPoint);


            // Update the custom cursor's anchored position to match the mouse position
            customCursor.GetComponent<RectTransform>().anchoredPosition = localPoint;
            // Adjust the position to center the cursor
            customCursor.GetComponent<RectTransform>().anchoredPosition += new Vector2(32, -44);

            if (sceneManager.sceneType == MySceneManager.SceneType.COMBAT && roundManager.currentPhase == RoundManager.TurnPhase.targetingATK) // if the targeting phase is active
            {
                ccImage.sprite = blade_cursor; // Change to hovered cursor sprite
                // Adjust the position to center the cursor
                customCursor.GetComponent<RectTransform>().anchoredPosition += new Vector2(-32, 44);
                updateCursorScale(blade_cursorSize); // change the size of the cursor
            }
            // else if (roundManager.currentPhase == RoundManager.TurnPhase.targetingSKILL) // if the targeting phase is active
            // {

            // }
            else if (Input.GetMouseButton(0)) // left click
            {


                ccImage.sprite = onClick_cursor; // Change to clicked cursor sprite
                updateCursorScale(onClick_cursorSize); // change the size of the cursor


            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (holdable != null) { PressAndHoldTarget.StopHoldGlobal(); }
                ccImage.sprite = base_cursor; // Change back to default cursor sprite
                updateCursorScale(base_cursorSize); // change the size of the cursor
            }
            else
            {

                ccImage.sprite = base_cursor; // Change back to default cursor sprite
                updateCursorScale(base_cursorSize); // change the size of the cursor
            }


        }






    }

    // delas with clicking and holing a press and hold object
    public void startPAHTHolding()
    {
        //  Start hold if any
        if (holdable != null)
        {
            if (holdingCoroutine != null) StopCoroutine(holdingCoroutine);
            holdingCoroutine = StartCoroutine(startPAHTHoldingCoroutine());
        }

    }

    private IEnumerator startPAHTHoldingCoroutine()
    {
        print("coroutine started");
        while (holdTime <= pahtDuration)
        {
            holdTime += Time.deltaTime;
            paht_circle.fillAmount = holdTime / pahtDuration;

            yield return null;
        }
        holdable.gotCompleted();
        stopPAHTHolding(true);
        
    }
    public void stopPAHTHolding(bool instantanious = false)
    {
        if ( holdingCoroutine != null) StopCoroutine(holdingCoroutine);

        if (instantanious)
        {
            holdable = null;
            paht_circle.fillAmount = 0;
            holdTime = 0f;
        }
        else
        {
            holdingCoroutine = StartCoroutine(stopPAHTHoldingCoroutine());
        }

        
    }
    private IEnumerator stopPAHTHoldingCoroutine()
    {
        
        holdable = null;
        while (holdTime >= 0f)
        {
            holdTime -= 2f * Time.deltaTime;
            paht_circle.fillAmount = holdTime / pahtDuration;
            yield return null;
        }

    }



    private IEnumerator LoadingDelay()
    {
        yield return null; // wait 1 frame
        GameObject mainCanvasObj = GameObject.FindGameObjectWithTag("MainCanvas");
        if (mainCanvasObj != null)
        {
            canvas = mainCanvasObj.GetComponent<Canvas>();
        }
    }

    private void updateCursorScale(float scale)
    {
        customCursor.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
    }

    public void setCursor(bool visible, bool locked)
    {
        Cursor.visible = visible;
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }











}
