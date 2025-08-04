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

    private float xRotation = 0f; // pitch
    private float yRotation = 0f; // yaw

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }





    void LateUpdate()
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

        // Accumulate mouse movement
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        yRotation = NormalizeAngle(yRotation);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp - 30);

        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Vector3 desiredPos = rotation * Vector3.forward * distance;
        transform.position = lookTarget.position + desiredPos;
        transform.rotation = rotation;



    }
    
    // helper function to normilize angles, (fixes Y rotation flick when crossing 179/-179 degrees)
    float NormalizeAngle(float angle)
{
    while (angle > 180f) angle -= 360f;
    while (angle < -180f) angle += 360f;
    return angle;
}
}
