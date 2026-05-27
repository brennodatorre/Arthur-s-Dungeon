using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataClass 
{

    public string playerName;
    public int hp;
    public int maxHP;
    public int mp;
    public int maxMP;
    public int def;

    public DiceRoll baseATK;
    public int atkAdvantage;

    public int totalMainActions ;
    public int totalSupActions ;

    [Space (10)]
    [Header ("Player Attributes (0 = 1d20) ")]

    public int statusPoints;

    public int DEX ;
    public int ATLETISM ;
    public int AURA ;
    public int CHARISM ;
    public int LUCK ;
    public int INTUITION ;
    public int HEX ;
    public int INT ;
    public int WILL ;
    public int REFLEX ;
    public int PERSEPTION;
    public int FURTIVITY ;
    public int CONSTITUTION ;
    public int DOMINANCE ;


    [Space (10)]
    [Header ("Player Data")]
    
    [SerializeField] public List<Skill> skills ;
    [SerializeField] public List<Item> items ;
    [SerializeField] public List<StatusEffect> StatusEffects ;


    [Space(10)]
    [Header ("OR Data")]
    [Tooltip ("What determines the combat setter")]
    public int fableRecord ;
    public int fablePoints ;
    public int levelsBeat; // totoal levels beaten
    public int currentLevelsBeat; // levels beaten this death loop
    public int lives;
    public int death_counter ;
    public int kill_counter;


    [Space(10)]
    [Header ("Event Data")]
    public int Ilhas;




    [Space (20)]
    [Header ("Save Data")]
    public int HighestScore = 0;
    public List<string> fableItemsID = new List<string>();
    public List<string> fableSkillsID = new List<string>();
    public List<string> fableStatusEffectsID = new List<string>();




    public PlayerDataClass()
    {
        
    }



    

}
