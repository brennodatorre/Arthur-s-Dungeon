using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class camera_move : MonoBehaviour
{

    public GameObject player;      // The player's body 
    public Transform lookTarget;      // The point the camera looks at 

    public RenderTexture PixelRender;
    public RenderTexture RegularTexture;
    private int current_render_texture = 0; //  0 for PixelRender , 1 for RegularTexture

    public RawImage Render;

    public float sensitivity = 100f;
    public float distance = 5f;       // Distance from the lookTarget
    public float verticalClamp = 80f; // Max up/down angle

    private float xRotation = 0f;

    private float mouseX;
    private float mouseY;
    private Vector3 direction;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



 

    void LateUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Render.texture = Render.texture == PixelRender ? RegularTexture : PixelRender;

            if (current_render_texture == 0)
            {
                Camera.main.GetComponent<Camera>().targetTexture = RegularTexture;
                current_render_texture = 1;
            }
            else
            {
                Camera.main.GetComponent<Camera>().targetTexture = PixelRender;
                current_render_texture = 0;
            }
        }

        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;



        // Rotate the camera (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // Calculate camera rotation and position
        direction = Quaternion.Euler(xRotation, player.transform.eulerAngles.y, 0f) * Vector3.back;




        // Rotate the player body (yaw)
        player.transform.Rotate(Vector3.up * mouseX);

        // Set the camera position
        transform.position = player.GetComponent<player_move>().lookTarget.position + direction * distance;

        // set the camera rotation
        transform.LookAt(player.GetComponent<player_move>().lookTarget.position); 

         
        
      
    }
}
