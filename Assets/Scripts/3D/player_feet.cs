using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_feet : MonoBehaviour
{
    public bool isGrounded = false;

    // On the child collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            player_move.Instance.SetGrounded(true);
            isGrounded = true;
        }
        
            
            
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            player_move.Instance.SetGrounded(false);
            isGrounded = false;
        }
            
    }
}
