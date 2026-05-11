using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade_OnActive : MonoBehaviour
{
   public float fadeTime = 1f;

   public bool fadeInOnEnable = true;
   [Tooltip("obly fade out if called properly, not on actual OnDisable")]
   public bool fadeOutOnDisable = true;

    List<Component> visualComponents = new List<Component>();

    void Awake()
    {
        
        visualComponents.AddRange(GetComponentsInChildren<SpriteRenderer>());
        visualComponents.AddRange(GetComponentsInChildren<Image>());
        visualComponents.AddRange(GetComponentsInChildren<Text>());


    }

    void OnEnable()
   {
        if (!fadeInOnEnable) return;

        hideVisuals();

        
        



        StartCoroutine(FadeIn());
   }

   public void DisableFade()
   {
        if (!fadeOutOnDisable) return;
        Debug.Log("...");


      StartCoroutine(FadeOut());
   }

   private void hideVisuals()
   {
        foreach (var visual in visualComponents)
        {
            if (visual is SpriteRenderer sr)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            }
            else if (visual is Image img)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
            }
            else if (visual is Text txt)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
            }
        }
   }



    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);

            foreach (var visual in visualComponents)
            {
                if (visual is SpriteRenderer sr)
                {
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                }
                else if (visual is Image img)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
                }
                else if (visual is Text txt)
                {
                    txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, alpha);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure visuals are fully visible at the end
        foreach (var visual in visualComponents)
        {
            if (visual is SpriteRenderer sr)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
            else if (visual is Image img)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
            }
            else if (visual is Text txt)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1f);
            }
        }
    }


    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);

            foreach (var visual in visualComponents)
            {
                if (visual is SpriteRenderer sr)
                {
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                }
                else if (visual is Image img)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
                }
                else if (visual is Text txt)
                {
                    txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, alpha);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure visuals are fully invisible at the end
        foreach (var visual in visualComponents)
        {
            if (visual is SpriteRenderer sr)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            }
            else if (visual is Image img)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
            }
            else if (visual is Text txt)
            {
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
            }
        }


        this.gameObject.SetActive(false);
    }
}
