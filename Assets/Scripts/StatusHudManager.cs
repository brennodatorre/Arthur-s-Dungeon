using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusHudManager : MonoBehaviour
{

    public RoundManager roundManager;

    public Image hpBar;
    public Image mpBar;  



    // Update is called once per frame
    void Update()
    {
        // Update the health and mana bars based on the player's current stats
        Entity player = roundManager.player;
        hpBar.fillAmount = player.getHP() / player.getMaxHP();
        mpBar.fillAmount = player.getMP() / player.getMaxMP();
        
    }
}
