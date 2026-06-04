using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfterImage_effect_Mod_Addon : MonoBehaviour
{

    public Color color = Color.white;
    private float frequency;
    [Range(0,1)]public float frequencyScaler = 0.9f;

    public float afterImageLifetime = 0.5f;
    public int afterImageAmount = 3;

    private List<GameObject> afterImages = new List<GameObject>();

    private Vector3 lastPosition;


    void Start()
    {
        SetGhostPulling();
        SetFrequency();
        lastPosition = transform.position;

       
    }


    void Update()
    {
        if (Vector3.Distance(transform.position, lastPosition) > frequency)
        {
            SpawnAfterImage();
            lastPosition = transform.position;
        }

    } 

    /// <summary>
    /// Sets frequency based on how big is the object
    /// </summary>
    private void SetFrequency()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            frequency = Mathf.Max(spriteRenderer.bounds.size.x, spriteRenderer.bounds.size.y) * frequencyScaler;
        }
        else
        {
            Image image = GetComponent<Image>();
            if (image != null)
            {
                
                frequency = Mathf.Max(image.rectTransform.rect.width, image.rectTransform.rect.height) * frequencyScaler;
            }
            else
            {
                frequency = 0.1f; // default frequency if no SpriteRenderer or Image is found
            }
        }

        
    }


    /// <summary>
    /// Creates x ghost copies of the object with only Image/sprite components
    /// </summary>
    private void SetGhostPulling()
    {
        
        GameObject ghost = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
        ghost.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1); // set the ghost behind the original object in the hierarchy

        List<Component> components = new List<Component>(ghost.GetComponents<Component>());

        // removes all components except for SpriteRenderer and Image to create a ghost copy of the original
        foreach (Component component in components)
        {
            if (component is SpriteRenderer || component is Image || component is CanvasRenderer || component is RectTransform){}   
            else { Destroy(component); }

        }
        foreach (Transform child in ghost.transform) { Destroy(child.gameObject); } // remove all children of the ghost to avoid unwanted visuals

        
        ghost.SetActive(false); 
        ghost.name = gameObject.name + "_AfterImage_Ghost"; 


        // creates x ghosts and adds them to the afterImages list
        afterImages.Add(ghost);
        for (int i = 1; i < afterImageAmount; i++)
        {
            GameObject newGhost = Instantiate(ghost, transform.position, transform.rotation, transform.parent);
            newGhost.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1); // set the ghost behind the original object in the hierarchy
            newGhost.SetActive(false);
            newGhost.name = gameObject.name + "_AfterImage_Ghost_" + i;
            afterImages.Add(newGhost);
        }
        
    }

    /// <summary>
    /// Spawns a ghost copy of the object at the current position and starts fading it out over time
    /// </summary>
    private void SpawnAfterImage()
    {
       

        foreach (GameObject ghost in afterImages)
        {
            if (!ghost.activeInHierarchy)
            {
                if (ghost.GetComponent<SpriteRenderer>() != null){ 
                    ghost.GetComponent<SpriteRenderer>().color = color;
                    ghost.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
                }
                if (ghost.GetComponent<Image>() != null){
                    ghost.GetComponent<Image>().color = color;  
                    ghost.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                }


                ghost.transform.position = transform.position;
                ghost.transform.rotation = transform.rotation;
                ghost.SetActive(true);
                StartCoroutine(FadeOutAfterImage(ghost));
                
                return;
            }
        }

        // if all ghosts are active, spawn a new one and add it to the list
        GameObject newGhost = Instantiate(afterImages[0], transform.position, transform.rotation, transform.parent);
        newGhost.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1); // set the ghost behind the original object in the hierarchy
        newGhost.SetActive(true);
        newGhost.name = gameObject.name + "_AfterImage_Ghost_" + afterImages.Count;
        afterImages.Add(newGhost);
        StartCoroutine(FadeOutAfterImage(newGhost));



    }


    /// <summary>
    /// Fades out the ghost copy of the object over time by lerping its color alpha from 1 to 0, then deactivates it
    /// </summary>
    private IEnumerator FadeOutAfterImage(GameObject ghost)
    {
        float elapsedTime = 0f;
        SpriteRenderer spriteRenderer = ghost.GetComponent<SpriteRenderer>();
        Image image = ghost.GetComponent<Image>();

        while (elapsedTime < afterImageLifetime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / afterImageLifetime);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            }
            else if (image != null)
            {
                image.color = new Color(color.r, color.g, color.b, alpha);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ghost.SetActive(false);
    }

}
