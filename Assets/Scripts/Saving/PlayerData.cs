using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Entity;
using Unity.VisualScripting.Antlr3.Runtime;
using System.IO;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    public enum Trait { DEX, ATLETISM, AURA, CHARISM, LUCK, INTUITION, HEX, INT, WILL, REFLEX, PERSEPTION, FURTIVITY, CONSTITUTION, DOMINANCE };

   


    [SerializeField]
    private PlayerDataClass DEFAULT = new PlayerDataClass();
    [SerializeField]
    public PlayerDataClass jsonData = new PlayerDataClass();
    [SerializeField]
    private PlayerDataClass GameData = new PlayerDataClass();
    [HideInInspector]
    public bool isInTransition = false;


    public DungeonMemory dungeonMemory = new DungeonMemory();



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


        jsonData = loadJsonData(); // Load the JSON data into the GameData
        if (jsonData == null) jsonData = new PlayerDataClass();
        
        resetPlayerStatus(); // Initialize GameData with NewGameData
        


    }

    #region Major Data Management



    /// <summary>
    /// Saves data From Entity player into GameData
    /// </summary>
    /// <param name="player"></param>
    public void savePlayerData(Entity player)
    {
     
        ActiveEffectManager.RemoveAllEffects(player.activeStatusEffects);

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

        

        GameData.StatusEffects = new List<StatusEffect>(player.activeStatusEffects);

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

        player.activeStatusEffects = new List<StatusEffect>(GameData.StatusEffects);

        player.items = new List<Item>(GameData.items);
        
        Debug.Log("Player data loaded.");
    }

    /// <summary>
    /// Sets the GameData into the default NewGameData
    /// </summary>
    public void resetPlayerStatus()
    {
        CreateRuntimeCopy(DEFAULT);


        Debug.Log("Player data reset to new game data.");
    }


    /// <summary>
    /// Returns a new PlayerDataClass based on the default + 
    /// as a new GameData
    /// </summary>
    public PlayerDataClass CreateRuntimeCopy(PlayerDataClass _default)
    {
        


        
        
        GameData.hp = _default.hp + jsonData.hp;
        GameData.maxHP = _default.maxHP + jsonData.maxHP;
        GameData.mp = _default.mp + jsonData.mp;
        GameData.maxMP = _default.maxMP + jsonData.maxMP;
        GameData.def = _default.def + jsonData.def;
        GameData.baseATK = new DiceRoll ();
        GameData.baseATK.AddDice(_default.baseATK);
        GameData.baseATK.AddDice(jsonData.baseATK);

        GameData.atkAdvantage = _default.atkAdvantage + jsonData.atkAdvantage;
        GameData.totalMainActions = _default.totalMainActions + jsonData.totalMainActions;
        GameData.totalSupActions = _default.totalSupActions + jsonData.totalSupActions;
        GameData.DEX = _default.DEX + jsonData.DEX;
        GameData.ATLETISM = _default.ATLETISM + jsonData.ATLETISM;
        GameData.AURA = _default.AURA + jsonData.AURA;
        GameData.CHARISM = _default.CHARISM + jsonData.CHARISM;
        GameData.LUCK = _default.LUCK + jsonData.LUCK;
        GameData.INTUITION = _default.INTUITION + jsonData.INTUITION;
        GameData.HEX = _default.HEX + jsonData.HEX;
        GameData.INT = _default.INT + jsonData.INT;
        GameData.WILL = _default.WILL + jsonData.WILL;
        GameData.REFLEX = _default.REFLEX + jsonData.REFLEX;
        GameData.PERSEPTION = _default.PERSEPTION + jsonData.PERSEPTION;
        GameData.FURTIVITY = _default.FURTIVITY + jsonData.FURTIVITY;
        GameData.CONSTITUTION = _default.CONSTITUTION + jsonData.CONSTITUTION;
        GameData.DOMINANCE = _default.DOMINANCE + jsonData.DOMINANCE;


        GameData.fablePoints = _default.fablePoints + jsonData.fablePoints;
        GameData.lives = _default.lives + jsonData.lives;
        GameData.Ilhas = _default.Ilhas + jsonData.Ilhas;



        GameData.skills = new List<Skill>();
        foreach (Skill skill in _default.skills){GameData.skills.Add(Instantiate(skill));}
        foreach (string s in jsonData.fableSkillsID) {
            Skill fableSkill = DatabaseManager.Instance.skillDatabase.GetSkillByID(s);
            if (fableSkill != null) {
                GameData.skills.Add(fableSkill);
            }
        }
        
        GameData.items = new List<Item>();
        foreach (Item item in _default.items){ GameData.items.Add(Instantiate(item));}
        foreach (string s in jsonData.fableItemsID) {
            Item fableItem = DatabaseManager.Instance.itemDatabase.GetItemByID(s);
            if (fableItem != null) {
                GameData.items.Add(fableItem);
            }
        }

        GameData.StatusEffects = new List<StatusEffect>();
        foreach (StatusEffect effect in _default.StatusEffects){GameData.StatusEffects.Add(Instantiate(effect));}
        foreach (string s in jsonData.fableStatusEffectsID) {
            StatusEffect fableEffect = DatabaseManager.Instance.statusEffectDatabase.GetStatusEffectByID(s);
            if (fableEffect != null) {
                GameData.StatusEffects.Add(fableEffect);
            }
        }


        return GameData;
    }


    public PlayerDataClass loadJsonData()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (!File.Exists(path))
        {
            Debug.Log("No save file found");
            return new PlayerDataClass();
        }

        string json = File.ReadAllText(path);

        PlayerDataClass data = JsonUtility.FromJson<PlayerDataClass>(json);

        data.skills = new List<Skill>();
        data.items = new List<Item>();
        data.StatusEffects = new List<StatusEffect>();

        return data;
    }

    public void saveJsonData()
    {
        string path = Application.persistentDataPath + "/save.json";

        string json = JsonUtility.ToJson(jsonData, true);

        File.WriteAllText(path, json);

        
    }


    public static void DeleteSave()
    {
        string path = Application.persistentDataPath + "/save.json";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file found.");
        }
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

    #endregion












   #region Player Data Manipulation
   
   /// <summary>
   /// Rolls a trait check using the specified trait, 
   /// using the player's current trait level as the dice pool
   /// </summary>
   /// <param name="trait"></param>
   public int Roll(Trait trait)
    {
        int traitLevel = GetTrait(trait);
        return DiceRoll.rollTest(traitLevel);
    }     


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
    public void changeIlhas(int amount)
    {
        GameData.Ilhas += amount;
        if (GameData.Ilhas < 0) { GameData.Ilhas = 0;}
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
        if (GameData.levelsBeat > jsonData.HighestScore) { jsonData.HighestScore = GameData.levelsBeat; }
    }
    public int GetHighestScore()
    {
        return jsonData.HighestScore;
    }
    public void incrementKillCounter()
    {
        GameData.kill_counter += 1;
        
    }
    public void incrementDeathCounter()
    {
        GameData.death_counter += 1;

    }


    public void healPlayer(int amount)
    {
        GameData.hp += amount;
        if (GameData.hp > GameData.maxHP) { GameData.hp = GameData.maxHP; }
    }
    public void changeMP(int amount)
    {
        GameData.mp += amount;
        if (GameData.mp > GameData.maxMP) { GameData.mp = GameData.maxMP; }
        else if (GameData.mp < 0) { GameData.mp = 0; }
    }
    public void takeTrueDamage(int amount)
    {
        GameData.hp -= amount;
        if (GameData.hp < 0) { GameData.hp = 0; }
    }
    
    public void changeMaxHP (int amount)
    {
        GameData.maxHP += amount;
        if (GameData.maxHP < 1) { GameData.maxHP = 1; }
        if (GameData.hp > GameData.maxHP) { GameData.hp = GameData.maxHP; }
        if (amount > 0) { healPlayer(amount); } 
        
    }
    public void changeMaxMP (int amount)
    {
        GameData.maxMP += amount;
        if (GameData.maxMP < 0) { GameData.maxMP = 0; }
        if (GameData.mp > GameData.maxMP) { GameData.mp = GameData.maxMP; }
        if (amount > 0) { changeMP(amount); } 
        
    }
    public void changeDEF(int amount)
    {
        GameData.def += amount;
        
    }

    public void changeStatusPoints(int amount)
    {
        GameData.statusPoints += amount;
        if (GameData.statusPoints < 0) { Debug.LogWarning("Status points cannot be negative!"); GameData.statusPoints = 0; }
    }

 #region getters
    public int getMaxHP()
        {
            return GameData.maxHP;
        }
    public int getMaxMP()
    {
        return GameData.maxMP;
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
    public int getCurrentHP()
    {
        return GameData.hp;
    }
    public int getCurrentMP()
    {
        return GameData.mp;
    }
    public int getDEF()
    {
        return GameData.def;
    }
    /// <summary>
    /// Returns an item in the player's inventory that has the specified property, or null if no such item exists
    /// </summary>
    public List<Item> hasItemsWithProperty(Item.ItemProperty property)
    {
        List<Item> itemsWithProperty = new List<Item>();
        foreach (Item item in GameData.items)
        {
            if (item.HasProperty(property))
            {
                itemsWithProperty.Add(item);
            }
        }
        return itemsWithProperty; // Return all items with the specified property
    }    
    
    public List<StatusEffect> getActiveSkillEffects()
    {
        return GameData.StatusEffects;
    }

    public int GetTrait(Trait trait)
    {
        int result = 0;

        switch (trait)
        {
            case Trait.DEX:
                result = GameData.DEX;
                break;
            case Trait.ATLETISM:
                result = GameData.ATLETISM;
                break;
            case Trait.AURA:
                result = GameData.AURA;
                break;
            case Trait.CHARISM:
                result = GameData.CHARISM;
                break;
            case Trait.LUCK:
                result = GameData.LUCK;
                break;
            case Trait.INTUITION:
                result = GameData.INTUITION;
                break;
            case Trait.HEX:
                result = GameData.HEX;
                break;
            case Trait.INT:
                result = GameData.INT;
                break;
            case Trait.WILL:
                result = GameData.WILL;
                break;
            case Trait.REFLEX:
                result = GameData.REFLEX;
                break;
            case Trait.PERSEPTION:
                result = GameData.PERSEPTION;
                break;
            case Trait.FURTIVITY:
                result = GameData.FURTIVITY;
                break;
            case Trait.CONSTITUTION:
                result = GameData.CONSTITUTION;
                break;
            case Trait.DOMINANCE:
                result = GameData.DOMINANCE;
                break; 
        }

        return result;

    } 



#endregion


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
        GameData.skills.Add(skill.Clone());
    }

    /// <summary>
    /// removes the skill from the player that matchs the id of the skill passed in
    /// </summary>
    public void RemoveSkill(Skill skill)
    {
        foreach (Skill skl in GameData.skills)
        {
            if (skl.skillID == skill.skillID)
            {
                GameData.skills.Remove(skl);
                break;
            }
        }

        
    }
    public bool HasSkill(Skill skill)
    {
        foreach (Skill skl in GameData.skills)
        {
            if (skl.skillID == skill.skillID)
            {
                return true;
            }
        }

        return false;
    }


    #endregion
}
