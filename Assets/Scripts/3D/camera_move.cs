using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class camera_move : MonoBehaviour
{

    public static camera_move Instance;

    public GameObject player;      // The player's body 
    public Transform lookTarget;      // The point the camera looks at 

    public RenderTexture PixelRender;
    public RenderTexture RegularTexture;
    private int current_render_texture = 0; //  0 for PixelRender , 1 for RegularTexture

    public RawImage Render;

    [Space(3)]
    public float sensitivity = 100f;
    public float distance = 5f;       // Distance from the lookTarget
    public float verticalClamp = 80f; // Max up/down angle
    public float smoothSpeed = 0.1f;  // Smoothing speed for camera movement
    public float smoothSpeedPos = 0.1f;  // Smoothing speed for camera movement

    private float xRotation = 0f; // pitch

    


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

        
    }


    void Start()
    {
        CursorManager.Instance.setCursor(false, true);
    }





    void Update()
    {
        // Unlocks cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Changes the render texture
        if (Input.GetKeyDown(KeyCode.R))
        {
            Render.texture = Render.texture == PixelRender ? RegularTexture : PixelRender;
            if (current_render_texture == 0)
            {
                Camera.main.targetTexture = RegularTexture;
                current_render_texture = 1;
            }
            else
            {
                Camera.main.targetTexture = PixelRender;
                current_render_texture = 0;
            }
        }

        


        //deals with camera rotation
        xRotation -= player_move.Instance.mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp - 30);
        Quaternion rotation = Quaternion.Euler(xRotation, player_move.Instance.transform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, smoothSpeed * Time.deltaTime);

        //deals with camera position
        Vector3 targetPos = lookTarget.position; 
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos + rotation * Vector3.forward * distance, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos;
        



    }


    













}
