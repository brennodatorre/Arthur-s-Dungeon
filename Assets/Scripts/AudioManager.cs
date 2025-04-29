using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{

    private AudioSource output;

    [SerializeField]public List<AudioClip> atk_sounds = new List<AudioClip>();
    [SerializeField]public List<AudioClip> hit_sounds = new List<AudioClip>(); 
    
    public AudioClip atk_button_sound;
    public AudioClip skill_button_sound;
    public AudioClip item_button_sound;
    public AudioClip run_button_sound;

    public AudioClip skill_unable_sound;

    private void Start()
    {
        output = GetComponent<AudioSource>();
    }

    //plays a random attack sound from the list
    public void PlayAttackSound()
    {
        int randomIndex = Random.Range(0, atk_sounds.Count);
        output.PlayOneShot(atk_sounds[randomIndex]);
    }

    public void PlayAtkButtonSound()
    {
        output.PlayOneShot(atk_button_sound);
    }

    public void PlaySkillButtonSound()
    {
        output.PlayOneShot(skill_button_sound);
    }

    public void PlayItemButtonSound()
    {
        output.PlayOneShot(item_button_sound);
    }

    public void PlayRunButtonSound()
    {
        output.PlayOneShot(run_button_sound);
    }


    public void PlaySound(AudioClip sound)
    {
        output.PlayOneShot(sound);
    }

}
