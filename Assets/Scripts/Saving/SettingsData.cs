using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsData : MonoBehaviour
{

    public static SettingsData Instance;

    public Volume postProcess;
    private DepthOfField dof;

    public bool isPaused = false;


    [Range (0,1)] public float globalVolume = .75f;

    public Slider globalVolumeSlider;



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

    }


    


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume(); else Pause();
            
        }

        if(isPaused)
        {
            globalVolume = globalVolumeSlider.value;
        }

    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        MySceneManager.Instance.inputBlocker.SetActive(true);
        dof.active = true;
        transform.GetChild(0).gameObject.SetActive(true);
        CursorManager.Instance.customCursor.transform.SetParent(transform.GetChild(0).transform, true);
        CursorManager.Instance.canvas = transform.GetChild(0).GetComponent<Canvas>();
        
    }
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (!MySceneManager.Instance.isInTransition) MySceneManager.Instance.inputBlocker.SetActive(false);
        dof.active = false;
        transform.GetChild(0).gameObject.SetActive(false);
        CursorManager.Instance.canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>(); 
        CursorManager.Instance.customCursor.transform.SetParent(CursorManager.Instance.canvas.transform, true);
        
    
    }



  
}
