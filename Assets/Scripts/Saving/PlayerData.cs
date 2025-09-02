using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    //player starting status
    bool initialStatusSaved = false;

    int initialMaxHP;
    int initiaMaxMP;
    int initiaDef;
    int initialLives;

    DiceRoll initiaBaseATK;
    int initiaAtkAdvantage;
    int initiaMainActions;
    int initiaSupActions;


    int initiaDEXTREZA;
    int initiaATLETISMO;
    int initiaAURA;
    int initiaCHARISMA;
    int initiaSORTE;
    int initiaINTUICAO;
    int initiaHEX;
    int initiaASTUCIA;
    int initiaVONTADE;
    int initiaREFLEXOS;
    int initiaPERSEPCAO;
    int initiaFURTIVIDADE;
    int initiaCONSTITUICAO;
    int initiaDOMINANCIA;


    List<Skill> initialSkills = new List<Skill>();
    List<Skill> initialSkillsInstance = new List<Skill>();

    [Header("Player Deathless Data")]
    [SerializeField] public int actualMaxHP;
    [SerializeField] public int actualMaxMP;
    [SerializeField] public int actualDef;

    [SerializeField] public DiceRoll actualBaseATK = new DiceRoll();
    [SerializeField] public int actualAtkAdvantage = 0; //advantage for the attack roll
    [SerializeField] public int actualMainActions = 1;
    [SerializeField] public int actualSupActions = 1;

    [Space]
    [SerializeField] public int actualDEXTREZA = 1;
    [SerializeField] public int actualATLETISMO = 1;
    [SerializeField] public int actualAURA = 1;
    [SerializeField] public int actualCHARISMA = 1;
    [SerializeField] public int actualSORTE = 1;
    [SerializeField] public int actualINTUICAO = 1;
    [SerializeField] public int actualHEX = 1;
    [SerializeField] public int actualASTUCIA = 1;
    [SerializeField] public int actualVONTADE = 1;
    [SerializeField] public int actualREFLEXOS = 1;
    [SerializeField] public int actualPERSEPCAO = 1;
    [SerializeField] public int actualFURTIVIDADE = 1;
    [SerializeField] public int actualCONSTITUICAO = 1;
    [SerializeField] public int actualDOMINANCIA = 1;


    [Space(5)]
    [SerializeField] private int fableRecord = 0;
    [SerializeField] private int fablePoints = 0;
    public int levelsBeat; // totoal levels beaten
    public int currentLevelsBeat; // levels beaten this death loop
    public int lives;
    public int death_counter = 0;
    public int kill_counter = 0;

    public bool isDead = false;





    [Space(10)]
    [Header("Player Floating Data")]
    [SerializeField] public string playerName;
    [SerializeField] private int hp;
    [SerializeField] private int maxHP;
    [SerializeField] private int mp;
    [SerializeField] private int maxMP;
    [SerializeField] public int def;

    [SerializeField] public DiceRoll baseATK = new DiceRoll();
    [SerializeField] public int atkAdvantage = 0;

    [SerializeField] public int totalMainActions = 1;
    [SerializeField] public int totalSupActions = 1;

    [Space(3)]
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




    [SerializeField] public List<Skill> skills = new List<Skill>();
    [HideInInspector] public List<Skill> skillsInstance = new List<Skill>();
    [SerializeField] public List<StatusEffect> activeSkillEffects = new List<StatusEffect>();


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

    void Start()
    {
        // saves initial values for true death
        if (!initialStatusSaved) { saveInitialStatus();  initialStatusSaved = true; }
    }

    public void savePlayerData(Entity player)
    {


        // Copy over the data
        playerName = player.name;
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
        activeSkillEffects = new List<StatusEffect>();

        Debug.Log("Player data saved.");
    }

    public void LoadPlayerData(Entity player)
    {
        // Paste the saved data into the player
        player.name = playerName;
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
        player.activeSkillEffects = new List<StatusEffect>(activeSkillEffects);

        Debug.Log("Player data loaded.");
    }

    //resets the player floating data to their actual data
    public void revitalizePlayer()
    {


        activeSkillEffects.Clear(); // Clear active skill effects
        isDead = false; //



        hp = actualMaxHP;
        maxHP = actualMaxHP;
        mp = actualMaxMP;
        maxMP = actualMaxMP;
        def = actualDef;
        baseATK = actualBaseATK;
        atkAdvantage = actualAtkAdvantage;

        totalMainActions = actualMainActions;
        totalSupActions = actualSupActions;

        DEXTREZA = actualDEXTREZA;
        ATLETISMO = actualATLETISMO;
        AURA = actualAURA;
        CHARISMA = actualCHARISMA;
        SORTE = actualSORTE;
        INTUICAO = actualINTUICAO;
        HEX = actualHEX;
        ASTUCIA = actualASTUCIA;
        VONTADE = actualVONTADE;
        REFLEXOS = actualREFLEXOS;
        PERSEPCAO = actualPERSEPCAO;
        FURTIVIDADE = actualFURTIVIDADE;
        CONSTITUICAO = actualCONSTITUICAO;
        DOMINANCIA = actualDOMINANCIA;


        Debug.Log("Player data revitalized.");
    }


    //deals prevents fableRecords from being decreased 
    public void addFablePoints(int amount)
    {
        if (amount < 0)
        { throw new System.ArgumentOutOfRangeException(nameof(amount), "Value must be positive."); }

        fablePoints += amount;
        fableRecord += amount;
    }
    ////...
    public void loseFablePoints(int amount)
    {
        if (amount < 0)
        { throw new System.ArgumentOutOfRangeException(nameof(amount), "Value must be positive."); }


        fablePoints -= amount;

    }
    ///...
    public int getCurrentFablePoints()
    {
        return fablePoints;
    }
    ///... 
    public int getFableRecord()
    {
        return fableRecord;
    }


    private void saveInitialStatus()
    {

        activeSkillEffects.Clear(); // Clear active skill effects
        isDead = false; //
        initialLives = lives;

        initialMaxHP = actualMaxHP;
        initiaMaxMP = actualMaxMP;
        initiaDef = actualDef;

        initiaBaseATK = new DiceRoll(actualBaseATK); // if DiceRoll has a copy constructor
        initiaAtkAdvantage = actualAtkAdvantage;
        initiaMainActions = actualMainActions;
        initiaSupActions = actualSupActions;

        initiaDEXTREZA = actualDEXTREZA;
        initiaATLETISMO = actualATLETISMO;
        initiaAURA = actualAURA;
        initiaCHARISMA = actualCHARISMA;
        initiaSORTE = actualSORTE;
        initiaINTUICAO = actualINTUICAO;
        initiaHEX = actualHEX;
        initiaASTUCIA = actualASTUCIA;
        initiaVONTADE = actualVONTADE;
        initiaREFLEXOS = actualREFLEXOS;
        initiaPERSEPCAO = actualPERSEPCAO;
        initiaFURTIVIDADE = actualFURTIVIDADE;
        initiaCONSTITUICAO = actualCONSTITUICAO;
        initiaDOMINANCIA = actualDOMINANCIA;


        initialSkills = new List<Skill>(skills);
        initialSkillsInstance = new List<Skill>(skillsInstance);



        Debug.Log("Player initial data saved.");
    }

    //resets for initial informartion
    public void resetPlayerStatus()
    {

        activeSkillEffects.Clear();
        isDead = false;
        lives = initialLives;
        fableRecord = 0;
        fablePoints = 0;
        levelsBeat = 0; // totoal levels beaten
        currentLevelsBeat = 0; // levels beaten this death loop
        death_counter = 0;
        kill_counter = 0;



        // --- initial → actual ---
        actualMaxHP = initialMaxHP;
        actualMaxMP = initiaMaxMP;
        actualDef = initiaDef;
        actualBaseATK = new DiceRoll(initiaBaseATK); // deep copy
        actualAtkAdvantage = initiaAtkAdvantage;
        actualMainActions = initiaMainActions;
        actualSupActions = initiaSupActions;

        actualDEXTREZA = initiaDEXTREZA;
        actualATLETISMO = initiaATLETISMO;
        actualAURA = initiaAURA;
        actualCHARISMA = initiaCHARISMA;
        actualSORTE = initiaSORTE;
        actualINTUICAO = initiaINTUICAO;
        actualHEX = initiaHEX;
        actualASTUCIA = initiaASTUCIA;
        actualVONTADE = initiaVONTADE;
        actualREFLEXOS = initiaREFLEXOS;
        actualPERSEPCAO = initiaPERSEPCAO;
        actualFURTIVIDADE = initiaFURTIVIDADE;
        actualCONSTITUICAO = initiaCONSTITUICAO;
        actualDOMINANCIA = initiaDOMINANCIA;

        skills = new List<Skill>(initialSkills);
        skillsInstance = new List<Skill>(initialSkillsInstance);

        // --- refresh floating from actual ---
        revitalizePlayer();

        Debug.Log("Player actual data reset to initial stats.");
    }

}
