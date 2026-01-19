using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    /// 
    /// Newgame context (OR context ( Current Life context ) )
    /// 
    [System.Serializable]
    public struct PlayerDataStruct
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
        [Header ("Player Attributes")]
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



    }


    [SerializeField]
    private PlayerDataStruct NewGameData = new PlayerDataStruct();
    [SerializeField]
    private PlayerDataStruct GameData = new PlayerDataStruct();






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

        GameData = NewGameData;
        


    }

    /// <summary>
    /// Saves data From Entity player into GameData
    /// </summary>
    /// <param name="player"></param>
    public void savePlayerData(Entity player)
    {
     
        ActiveEffectManager.RemoveAllEffects(player.activeSkillEffects);

        // Copy over the data
        GameData.playerName = player.name;
        GameData.hp = player.getHP();
        GameData.maxHP = player.getMaxHP();
        GameData.mp = player.getMP();
        GameData.maxMP = player.getMaxMP();
        GameData.def = player.def;
        GameData.baseATK = player.baseATK;
        GameData.atkAdvantage = player.atkAdvantage;
        GameData.totalMainActions = player.totalMainActions;
        GameData.totalSupActions = player.totalSupActions;
        GameData.DEX = player.DEX;

        GameData.ATLETISM= player.ATLETISM;
        GameData.AURA = player.AURA;
        GameData.CHARISM= player.CHARISM;
        GameData.LUCK = player.LUCK;
        GameData.INTUITION = player.INTUITION;
        GameData.HEX = player.HEX;
        GameData.INT = player.INT;
        GameData.WILL = player.WILL;
        GameData.REFLEX = player.REFLEX;
        GameData.PERSEPTION = player.PERSEPTION;
        GameData.FURTIVITY= player.FURTIVITY;
        GameData.CONSTITUTION = player.CONSTITUTION;
        GameData.DOMINANCE = player.DOMINANCE;
        


        
        GameData.skills = new List<Skill>(player.skills);

        

        GameData.StatusEffects = new List<StatusEffect>(player.activeSkillEffects);

        GameData.items = new List<Item>(player.items);
        

        Debug.Log("Player data saved.");
    }

    /// <summary>
    /// Saves data From GameData into Entity player
    /// </summary>
    /// <param name="player"></param>
    public void LoadPlayerData(Entity player)
    {
        // Paste the saved data into the player
        player.setHP(GameData.hp);
        player.setMaxHP(GameData.maxHP);
        player.setMP(GameData.mp);
        player.setMaxMP(GameData.maxMP);
        player.def = GameData.def;
        player.baseATK = GameData.baseATK;
        player.atkAdvantage = GameData.atkAdvantage;
        player.totalMainActions = GameData.totalMainActions;
        player.totalSupActions = GameData.totalSupActions;
        player.DEX = GameData.DEX;
        player.ATLETISM = GameData.ATLETISM;
        player.AURA = GameData.AURA;
        player.CHARISM = GameData.CHARISM;
        player.LUCK = GameData.LUCK;
        player.INTUITION = GameData.INTUITION;
        player.HEX = GameData.HEX;
        player.INT = GameData.INT;
        player.WILL = GameData.WILL;
        player.REFLEX = GameData.REFLEX;
        player.PERSEPTION = GameData.PERSEPTION;
        player.FURTIVITY = GameData.FURTIVITY;
        player.CONSTITUTION = GameData.CONSTITUTION;
        player.DOMINANCE = GameData.DOMINANCE;
        


        
        player.skills = new List<Skill>(GameData.skills);

        player.activeSkillEffects = new List<StatusEffect>(GameData.StatusEffects);

        player.items = new List<Item>(GameData.items);
        
        Debug.Log("Player data loaded.");
    }

    /// <summary>
    /// Sets the GameData into the default NewGameData
    /// </summary>
    public void resetPlayerStatus()
    {
        GameData = NewGameData;

        Debug.Log("Player data reset to new game data.");
    }


    /// <summary>
    /// Fully restores the player's HP and MP to max values
    /// </summary>
    public void revitalizePlayer()
    {
        GameData.hp = GameData.maxHP;
        GameData.mp = GameData.maxMP;


        Debug.Log("Player data revitalized.");
    }




///
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   
   



    //deals prevents fableRecords from being decreased 
    public void addFablePoints(int amount)
    {
        if (amount < 0)
        { throw new System.ArgumentOutOfRangeException(nameof(amount), "Value must be positive."); }

        GameData.fablePoints += amount;
        GameData.fableRecord += amount;
    }
    ////...
    public void loseFablePoints(int amount)
    {
        if (amount < 0)
        { throw new System.ArgumentOutOfRangeException(nameof(amount), "Value must be positive."); }


        GameData.fablePoints -= amount;

    }






   
    /// Modifies the player's lives by the specified amount, positive or negative
    /// </summary>
    public void setPlayerLives(int amount)
    {
        GameData.lives += amount;
    }
    /// <summary>
    /// Modifies the player's Ilhas by the specified amount, positive or negative
    /// </summary>
    public void setIlhas(int amount)
    {
        GameData.Ilhas += amount;
    }
    public void setMaxHP(int amount)
    {
        GameData.maxHP += amount;
    }
    public void setMaxMP(int amount)
    {
        GameData.maxMP += amount;
    }
    public void setDEF(int amount)
    {
        GameData.def += amount;
    }
    ///
    public void resetLevelsBeat()
    {
        GameData.currentLevelsBeat = 0;
    }
    public void incrementLevelsBeat()
    {
        GameData.levelsBeat += 1;
        GameData.currentLevelsBeat += 1;
    }
    public void incrementKillCounter()
    {
        GameData.kill_counter += 1;
        
    }
    public void incrementDeathCounter()
    {
        GameData.death_counter += 1;

    }




    public int getCurrentFablePoints()
    {
        return GameData.fablePoints;
    }
    public int getFableRecord()
    {
        return GameData.fableRecord;
    }
    public int getLives()
    {
        return GameData.lives;
    }
    public int getDeathCounter()
    {
        return GameData.death_counter;
    }
    public int getIlhas()
    {
        return GameData.Ilhas;
    }
    public DiceRoll getBaseATK()
    {
        return GameData.baseATK;
    }
    public int getLevelsBeat()
    {
        return GameData.levelsBeat;
    }
    public List<Skill> getSkills()
    {
        return GameData.skills;
    }
    public List<Item> getItems()
    {
        return GameData.items;
    }
    public List<StatusEffect> getActiveSkillEffects()
    {
        return GameData.StatusEffects;
    }

    



    public void addItemToInventory(Item item)
    {
        GameData.items.Add(Instantiate(item));
    }
    public void RemoveItem(Item item)
    {
        GameData.items.Remove(item);
    }


    public void addSkill(Skill skill)
    {
        GameData.skills.Add(Instantiate(skill));
    }
    public void RemoveSkill(Skill skill)
    {
        GameData.skills.Remove(skill);
    }





}
