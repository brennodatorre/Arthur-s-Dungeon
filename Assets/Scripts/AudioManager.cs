using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Entity player;

    public AudioSource SFXoutput;
   
    public AudioSource ambienceOutput;



    public int[] atk_levels;
    [SerializeField] public List<AudioClip> atk_sounds = new List<AudioClip>();
    //[SerializeField] public List<AudioClip> hit_sounds = new List<AudioClip>();
    public AudioClip atk_equal_sound;




    #region Sound Clips

    [Space(10)]
    [Header("Sound Clips:")]
    public AudioClip atk_button_sound;
    public AudioClip skill_button_sound;
    public AudioClip item_button_sound;
    public AudioClip run_button_sound;

    [Space(7)]
    public AudioClip qteSpeedSound;
    public AudioClip qteFail;
    public AudioClip qteSucess;

    [Space(7)]
    public AudioClip skill_unable_sound;
    public AudioClip death_sound;
    public AudioClip skill_page_select_sound;

    [Space(7)]
    public AudioClip statusEffect_end_sound;

    #endregion


    private float originalSFXVolume;
    private float originalAmbienceVolume;


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

        originalSFXVolume = SFXoutput.volume;
        originalAmbienceVolume = ambienceOutput.volume;

    }

    private void Update()
    {
        
        if (MySceneManager.Instance.isInTransition) {return;}
        setVolume();


        // adjjusts the player heartbeat sound based on the player's HP, while player is in combat
        if (player != null)
        {
            ambienceOutput.pitch = Mathf.Lerp(2.5f, .5f, (float)player.getHP() / (float)player.getMaxHP());
        }
    }


    public void PlaySound(AudioClip sound)
    {
        SFXoutput.PlayOneShot(sound);
    }








    //plays a random attack sound from the list
    public void PlayAttackSound(float damage)
    {
        int index = 0;

        //gets the level of the damage
        for (int i = 0; i < atk_levels.Length; i++)
        {
            if (damage <= atk_levels[i]) { break; } else { index++; }
        }

        if (index > atk_levels.Length - 1) { index--; }
        //Debug.Log("index " +index );

        SFXoutput.PlayOneShot(atk_sounds[index]);
        //Debug.Log(atk_sounds[index].name);
    }

    public void PlayAtkButtonSound()
    {
        SFXoutput.PlayOneShot(atk_button_sound);
    }

    public void PlaySkillButtonSound()
    {
        SFXoutput.PlayOneShot(skill_button_sound);
    }

    public void PlayItemButtonSound()
    {
        SFXoutput.PlayOneShot(item_button_sound);
    }

    public void PlayRunButtonSound()
    {
        SFXoutput.PlayOneShot(run_button_sound);
    }


    

    // fix this
    // public void PlaySoundWithRandomPitch(AudioClip sound)
    // {
    //     SFXoutput.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
    //     SFXoutput.PlayOneShot(sound);
    //     //SFXoutput.pitch = 1f; // reset pitch to normal after playing
    // }

    public void PlaySkillPageSelectdSound()
    {
        SFXoutput.PlayOneShot(skill_page_select_sound);
    }



    public AudioSource PlayQTESoundWithProgressivePitch(AudioClip sound, System.Func<float>  t, float speed, System.Func<bool> isRunning = null)
    {
        AudioSource temp = gameObject.AddComponent<AudioSource>();

        StartCoroutine(PlayPitchOverTime(temp, sound, t, speed, isRunning));

        return temp;
    }

    private IEnumerator PlayPitchOverTime(AudioSource src, AudioClip clip, System.Func<float> t, float speed, System.Func<bool> isRunning = null)
    {
        if (src == null || clip == null || speed <= 0f) yield break;

        float qteDuration = 1f / speed;
        float avgPitch = clip.length / qteDuration;
        float startPitch = Mathf.Clamp(avgPitch * 0.5f, 0.1f, 3f);
        float endPitch   = Mathf.Clamp(avgPitch * 1.5f, 0.1f, 3f);
        if (Mathf.Abs(((startPitch + endPitch) * 0.5f) - avgPitch) > 0.01f)
            startPitch = endPitch = Mathf.Clamp(avgPitch, 0.1f, 3f);

        src.clip = clip;
        src.pitch = startPitch;
        src.Play();

        while ((isRunning == null || isRunning()) && src != null)
        {
            float tt = Mathf.Clamp01(t());        
            src.pitch = Mathf.Lerp(startPitch, endPitch, tt);
            if (tt >= 1f) break;                   
            yield return null;
        }

        if (src != null) { src.Stop(); Destroy(src); }
    }

    /// <summary>
    /// Scales the volume of the game based on the SettingsData as the percentage 
    /// </summary> <summary>
    public void setVolume()
    {
        SFXoutput.volume = (originalSFXVolume / 100) * SettingsData.Instance.globalVolume * 100;
        ambienceOutput.volume = (originalAmbienceVolume / 100) * SettingsData.Instance.globalVolume * 100;
    }

}
