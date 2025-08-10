using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class player_move : MonoBehaviour
{

    private Rigidbody rb;
    public float speed = 5f;
    public float sprintSpeed = 5f;
    public Transform orientation;
    public Transform lookTarget; 
    public Transform Cam; // Reference to the camera transform

    public bool isSprinting = false;

   
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

        if (Input.GetKey(KeyCode.LeftShift))
        { //sprint
            isSprinting = true;
            rb.AddForce(moveDirection * (speed + sprintSpeed) * Time.deltaTime, ForceMode.VelocityChange);
        }
        else
        { //walk
            isSprinting = false;
            rb.AddForce(moveDirection * speed * Time.deltaTime, ForceMode.VelocityChange);
        }
        

        

        // Rotate the player to face the camera direction
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Cam.GetComponent<camera_move>().sensitivity * Time.deltaTime);

        Quaternion CamRotation = Cam.rotation;
        CamRotation.x = 0f;
        CamRotation.z = 0f;

        transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
}
}
