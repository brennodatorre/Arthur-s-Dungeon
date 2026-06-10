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

   public bool isFading;

    Dictionary<Component, float> visualComponents = new Dictionary<Component, float>();
     

    void Awake()
    {
        
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            visualComponents.Add(sr, sr.color.a);

        foreach (var img in GetComponentsInChildren<Image>())
            visualComponents.Add(img, img.color.a);

        foreach (var txt in GetComponentsInChildren<Text>())
            visualComponents.Add(txt, txt.color.a);


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
        foreach (var pair in visualComponents)
        {
            Component visual = pair.Key;

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
        isFading = true;

        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            float t = elapsedTime / fadeTime;

            foreach (var pair in visualComponents)
            {
                Component visual = pair.Key;
                float originalAlpha = pair.Value;

                float alpha = Mathf.Lerp(0f, originalAlpha, t);

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
        foreach (var pair in visualComponents)
        {
            Component visual = pair.Key;
            float originalAlpha = pair.Value;

            if (visual is SpriteRenderer sr)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, originalAlpha);

            else if (visual is Image img)
                img.color = new Color(img.color.r, img.color.g, img.color.b, originalAlpha);

            else if (visual is Text txt)
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, originalAlpha);
        }

        isFading = false;
    }


    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            float t = elapsedTime / fadeTime;

            foreach (var pair in visualComponents)
            {
                Component visual = pair.Key;
                float originalAlpha = pair.Value;

                float alpha = Mathf.Lerp(originalAlpha, 0f, t);

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
        foreach (var pair in visualComponents)
        {
            Component visual = pair.Key;

            if (visual is SpriteRenderer sr)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

            else if (visual is Image img)
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);

            else if (visual is Text txt)
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
        }


        this.gameObject.SetActive(false);
    }
}
