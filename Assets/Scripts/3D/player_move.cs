using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour
{

    private Rigidbody rb;
    public float speed = 5f;
    public Transform orientation;

    public Vector3 cacheTargetPosition; // Cache the target position for camera movement

    public Transform lookTarget; 

    public Transform Cam; // Reference to the camera transform

   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {

        float hor = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float ver = Input.GetAxis("Vertical");   // W/S or Up/Down arrows





        // Reset horizontal velocity only
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

        // Use orientation (yaw-only) instead of full transform
        Vector3 moveDirection = (orientation.forward * ver + orientation.right * hor).normalized;

        rb.AddForce(moveDirection * speed * Time.deltaTime, ForceMode.VelocityChange);

        cacheTargetPosition = lookTarget.position; // Cache the target position for camera movement


        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Cam.GetComponent<camera_move>().sensitivity * Time.deltaTime);


            Quaternion CamRotation = Cam.rotation;
            CamRotation.x = 0f;
            CamRotation.z = 0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
    }
}
