using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;

public class player_move : MonoBehaviour
{
    public static player_move Instance;

    private Rigidbody rb;
    private Vector3 prevPosition;

    // A/W/S/D inputs
    private float hor;
    private float ver;
    public float mouseX;
    public float mouseY;

    private bool isHoldingJump;
    private Vector3 moveDirection;

    public float speed = 5f;
    public float sprintSpeed = 5f;
    public float jumpForce = 5f; // Jump force, if needed
    public float fallingForceMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Space(7)]
    public Transform orientation;
    public Transform lookTarget;
    public Transform Cam; // Reference to the camera transform

    [Space(7)]
    public bool isSprinting = false;
    public bool isGrounded = false; // Check if the player is grounded
    public bool isFalling = false;




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
        rb = GetComponent<Rigidbody>();

        prevPosition = transform.position;

    }


    void FixedUpdate()
    {


        // Rotate the player 
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX  * Time.fixedDeltaTime , 0f));


        //checks if the player is falling
        if (prevPosition.y > transform.position.y && !isGrounded) { isFalling = true; }
        else { isFalling = false; }

        //saves position in memory
        prevPosition = transform.position;


        // starts applying extra falling force (only) in the frame the player starts falling 
        if (isFalling)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallingForceMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !isHoldingJump) // short hop
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }




        Vector3 targetVelocity = moveDirection * (isSprinting ? speed + sprintSpeed : speed);
        Vector3 velocityChange = targetVelocity - new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);


    }

    //here for more precise inputing
    void Update()
    {

        hor = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        ver = Input.GetAxis("Vertical");   // W/S or Up/Down arrows

        // Accumulate mouse movement
        mouseX = Input.GetAxis("Mouse X") * camera_move.Instance.sensitivity;  
        mouseY = Input.GetAxis("Mouse Y") * camera_move.Instance.sensitivity * Time.deltaTime;

        // Use orientation (yaw-only) instead of full transform to get the move direction
        moveDirection = (orientation.forward * ver + orientation.right * hor).normalized;

        // saves jump input 
        isHoldingJump = Input.GetKey(KeyCode.Space);
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        
        float yRotation = NormalizeAngle(mouseX);

        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        { //jump
            jump();

        }


    }




    private void jump()
    {
        // Reset vertical velocity before jumping for consistent jumps
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

        // helper function to normilize angles, (fixes Y rotation flick when crossing 179/-179 degrees)
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

}
