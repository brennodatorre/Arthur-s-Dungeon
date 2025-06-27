using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float reach = 3f; // how far the player can interact with objt

    Interactable currentInteractable; // currently interactable object

    // Update is called once per frame
    void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.interact();
            }
        }
    }

    private void CheckInteraction()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, reach))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Interactable newInt = hit.collider.GetComponent<Interactable>();

                if (currentInteractable && newInt != currentInteractable)
                {
                    //if the current int is different from the new int
                    currentInteractable.disableOutline();
                }

                if (newInt != currentInteractable)//
                {
                    if (newInt.enabled)
                    {
                        EnableCurrentInteractable(newInt);
                    }
                    else // if new interactble is not enabled
                    {
                        DisableCurrentInteractable();
                    }
                }
            }
        }
        else // if not interactble
        {
            DisableCurrentInteractable();
        }
    }

    void EnableCurrentInteractable(Interactable newInt)
    {
        currentInteractable = newInt;
        currentInteractable.enableOutline();

        //display the interaction text
        hudController.instance.EnableInteractionText(currentInteractable.message);
    }
    
    void DisableCurrentInteractable()
    {
        //hides interaction text
        hudController.instance.DesableInteractionText();

        if (currentInteractable == null) return;

        currentInteractable.disableOutline();
        currentInteractable = null;
        
    }
}
