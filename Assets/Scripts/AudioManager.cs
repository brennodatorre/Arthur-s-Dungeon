using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Entity player;

    public AudioSource SFXoutput;
    public AudioSource ambienceOutput;

    public int[] atk_levels;
    [SerializeField]public List<AudioClip> atk_sounds = new List<AudioClip>();
    [SerializeField]public List<AudioClip> hit_sounds = new List<AudioClip>();
    public AudioClip atk_equal_sound;


    [Space]
    public AudioClip atk_button_sound;
    public AudioClip skill_button_sound;
    public AudioClip item_button_sound;
    public AudioClip run_button_sound;

    public AudioClip skill_unable_sound;
    public AudioClip death_sound;
    public AudioClip skill_page_select_sound;





    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }

        
    }

    private void Update()
    {

        if (player != null){
            ambienceOutput.pitch = Mathf.Lerp(2.5f, .5f, player.getHP() / player.getMaxHP());
        }
    }

    //plays a random attack sound from the list
    public void PlayAttackSound(float damage)
    {
        int index = 0;

        //gets the level of the damage
        for (int i = 0; i < atk_levels.Length; i++){
            if (damage <= atk_levels[i] ) {break;} else {index++;}
        }
        
        if (index > atk_levels.Length-1) {index--;}
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


    public void PlaySound(AudioClip sound)
    {
        SFXoutput.PlayOneShot(sound);
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

}
