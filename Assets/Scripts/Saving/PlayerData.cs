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
    [SerializeField]public int currentMainActions = 1;
    [SerializeField]public int currentSupActions = 1;

    [SerializeField] public int totalMainActions = 1;
    [SerializeField] public int totalSupActions = 1;

    [Space]
    [SerializeField] public int DEXTREZA = 1;
    [SerializeField] public int ATLETISMO = 1;
    [SerializeField] public int AURA = 1;
    [SerializeField] public int CHARISMA = 1;
    [SerializeField] public int SORTE = 1;
    [SerializeField] public int INTUICAO = 1;
    [SerializeField] public int HEX = 1;
    [SerializeField] public int ASTUCIA = 1;
    [SerializeField] public int VONTADE = 1;
    [SerializeField] public int REFLEXOS = 1;
    [SerializeField] public int PERSEPCAO = 1;
    [SerializeField] public int FURTIVIDADE = 1;
    [SerializeField] public int CONSTITUICAO = 1;
    [SerializeField] public int DOMINANCIA = 1;


    [Space]
    [SerializeField] public int fablePoints = 0;
    public int death_counter = 0;
    public int kill_counter = 0;
    



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
            totalMainActions = player.totalMainActions;
            totalSupActions = player.totalSupActions;
            DEXTREZA = player.DEXTREZA;
            ATLETISMO = player.ATLETISMO;
            AURA = player.AURA;
            CHARISMA = player.CHARISMA;
            SORTE = player.SORTE;
            INTUICAO = player.INTUICAO;
            HEX = player.HEX;
            ASTUCIA = player.ASTUCIA;
            VONTADE = player.VONTADE;
            REFLEXOS = player.REFLEXOS;
            PERSEPCAO = player.PERSEPCAO;
            FURTIVIDADE = player.FURTIVIDADE;
            CONSTITUICAO = player.CONSTITUICAO;
            DOMINANCIA = player.DOMINANCIA;
            isDead = player.isDead;

            
            skills = new List<Skill>(player.skills);
            skillsInstance = new List<Skill>(player.skillsInstance);
            activeSkillEffects = new List<Skill>(player.activeSkillEffects);

            Debug.Log("Player data saved.");
    }

    public void LoadPlayerData(Entity player)
    {
        // Paste the saved data into the player
        player.name = name;
        player.setHP(hp);
        player.setMaxHP(maxHP);
        player.setMP(mp);
        player.setMaxMP(maxMP);
        player.def = def;
        player.baseATK = baseATK;
        player.atkAdvantage = atkAdvantage;
        player.DEXTREZA = DEXTREZA;
        player.ATLETISMO = ATLETISMO;
        player.AURA = AURA;
        player.CHARISMA = CHARISMA;
        player.SORTE = SORTE;
        player.INTUICAO = INTUICAO;
        player.HEX = HEX;
        player.ASTUCIA = ASTUCIA;
        player.VONTADE = VONTADE;
        player.REFLEXOS = REFLEXOS;
        player.PERSEPCAO = PERSEPCAO;
        player.FURTIVIDADE = FURTIVIDADE;
        player.CONSTITUICAO = CONSTITUICAO;
        player.DOMINANCIA = DOMINANCIA;
        player.totalMainActions = totalMainActions;
        player.totalSupActions = totalSupActions;
        player.isDead = isDead;

        
        player.skills = new List<Skill>(skills);
        player.skillsInstance = new List<Skill>(skillsInstance);
        player.activeSkillEffects = new List<Skill>(activeSkillEffects);

        Debug.Log("Player data loaded.");
    }


    public void revivePlayer()
    {
        // Paste the saved data into the player

        hp = maxHP;
        mp = maxMP;
        currentMainActions = totalMainActions;
        currentSupActions = totalSupActions;
        activeSkillEffects.Clear(); // Clear active skill effects

        isDead = false; //


        Debug.Log("Player data revived.");
    }

    





}
