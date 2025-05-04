using UnityEngine;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    
    [SerializeField] public new string name;
    [SerializeField] private float hp;
    [SerializeField] private float maxHP;
    [SerializeField] private float mp;
    [SerializeField] private float maxMP;
    [SerializeField] public float def;
    [SerializeField] public DiceRoll baseATK = new DiceRoll();
    [SerializeField] public int atkAdvantage = 0; //advantage for the attack roll
    [SerializeField] public int DEXTREZA = 1;
    [SerializeField] public int ATLETISMO = 1;


    public bool isDead = false;

   
    [SerializeField] public List<Skill> skills = new List<Skill>();
    [HideInInspector]public List<Skill> skillsInstance = new List<Skill>();
    [SerializeField] public List<Skill> activeSkillEffects = new List<Skill>();


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

    public void savePlayerData(Entity player){
    

            // Copy over the data
            name = player.name;
            hp = player.getHP();
            maxHP = player.getMaxHP();
            mp = player.getMP();
            maxMP = player.getMaxMP();
            def = player.def;
            baseATK = player.baseATK;
            atkAdvantage = player.atkAdvantage;
            DEXTREZA = player.DEXTREZA;
            ATLETISMO = player.ATLETISMO;
            isDead = player.isDead;

            // Deep copy the skills if needed
            skills = new List<Skill>(player.skills);
            skillsInstance = new List<Skill>(player.skillsInstance);
            activeSkillEffects = new List<Skill>(player.activeSkillEffects);

            Debug.Log("Player data saved.");
    }





}
