using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MapManager : MonoBehaviour
{

    public static MapManager Instance;
    public GameObject roomPrefab;

    private Canvas canvas;

    public int mapSizeX;
    public int mapSizeY;

    public float roomSpacing = 10f; 

    public float tileDistance;

    private bool isMapOpen = false;
    private bool mapIsSetUP = false;
    public Volume postProcess;
    private DepthOfField dof;


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

        postProcess = GameObject.FindWithTag("PostProcess").GetComponent<Volume>();
        postProcess.profile.TryGet(out dof);
        canvas = GetComponentInChildren<Canvas>();
    }

    void Start()
    {
        

        
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isMapOpen) CloseMap(); else OpenMap();
            
        }
    }


    private void setUpMap()
    {
        RectTransform prefabRect = roomPrefab.GetComponent<RectTransform>();
        float tileDistance = prefabRect.sizeDelta.x + roomSpacing;
        

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                GameObject newRoom = Instantiate(roomPrefab, canvas.transform);
                RectTransform rect = newRoom.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(
                    x * tileDistance,
                    y * tileDistance
                );
            }
        }

    }


    private void OpenMap()
    {
        
        Time.timeScale = 0f;
        MySceneManager.Instance.inputBlocker.SetActive(true);

        dof.active = true;

        transform.GetChild(0).gameObject.SetActive(true);
        CursorManager.Instance.customCursor.transform.SetParent(transform.GetChild(0).transform, true);
        isMapOpen = true;

        if (!mapIsSetUP)
        {
            canvas = GetComponentInChildren<Canvas>();
            setUpMap();
            mapIsSetUP = true;
        }
    }

    private void CloseMap()
    {
        
        Time.timeScale = 1f;

        if (!MySceneManager.Instance.isInTransition) MySceneManager.Instance.inputBlocker.SetActive(false);
        dof.active = false;

        transform.GetChild(0).gameObject.SetActive(false);
        CursorManager.Instance.customCursor.transform.SetParent(CursorManager.Instance.canvas.transform, true);
    
        isMapOpen = false;
    }
   

}
