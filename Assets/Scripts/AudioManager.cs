using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{

    public Entity player;

    public AudioSource SFXoutput;
    public AudioSource ambienceOutput;

    public int[] atk_levels;
    [SerializeField]public List<AudioClip> atk_sounds = new List<AudioClip>();
    [SerializeField]public List<AudioClip> hit_sounds = new List<AudioClip>(); 
    
    public AudioClip atk_button_sound;
    public AudioClip skill_button_sound;
    public AudioClip item_button_sound;
    public AudioClip run_button_sound;

    public AudioClip skill_unable_sound;
    public AudioClip death_sound;

    

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

}
