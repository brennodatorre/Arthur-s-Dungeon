using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class CombatSetter : MonoBehaviour
{


    public GameObject RandomLevelObj;
    public List<GameObject> allEnemyPrefabList = new List<GameObject>();
    [Space]
    public List<List<GameObject>> eLevelsLists = new List<List<GameObject>>(); //holds all the enemy list 
    public List<GameObject> enemyL1 = new List<GameObject>();
    public List<GameObject> enemyL2 = new List<GameObject>();
    public List<GameObject> enemyL3 = new List<GameObject>();
    public List<GameObject> enemyL4 = new List<GameObject>();
    public List<GameObject> enemyL5 = new List<GameObject>();
    public List<GameObject> enemyL6 = new List<GameObject>();


    [Space]
    public List<GameObject> enemyL999 = new List<GameObject>(); // enemy level list of outliers

    [Space(3)]
    public List<GameObject> possibleSpawnSpots = new List<GameObject>(); //used for the random generated fight 
    public List<GameObject> preMadeFight = new List<GameObject>(); // used for pre defined fight 

    [Space(10)]
    public int playerLevel;
    public List<GameObject> randomRoaster = new List<GameObject>();


    void Awake()
    {
        eLevelsLists.Add(enemyL6);
        eLevelsLists.Add(enemyL5);
        eLevelsLists.Add(enemyL4);
        eLevelsLists.Add(enemyL3);
        eLevelsLists.Add(enemyL2);
        eLevelsLists.Add(enemyL1);

        sortEnemyLists();
    }





    public void openLevel()
    {

        setRandomCombatRoaster();
        startRandomCombat();


    }



    public int getPlayerLevel()
    {
        int p = PlayerData.Instance.getFableRecord();

        // player level = ceil (ceil((x^4) / 500)^.6), 
        // where x are the amount of fable points the player has ever received
        int pLevel = Mathf.CeilToInt(Mathf.Pow(Mathf.CeilToInt(1f / 500f * Mathf.Pow(p, 4f)), 0.6f));

        if (pLevel == 0 ){ pLevel = 1; } //acounts for 0 FableRecord

        playerLevel = pLevel;

        return pLevel;

    }


    private void setRandomCombatRoaster()
    {
        int pLevel = getPlayerLevel();
        print(pLevel);

        /////////// really primitive, do improve/balance in the future \\\\\\\\\\\\\\

        int i = 6;
        foreach (List<GameObject> list in eLevelsLists)
        {

            if (list.Count == 0)
            {
                i--;
                continue;
            }

            //gets number of enemy of level i that can be spawnd based on player's level
            int mod = pLevel % i;
            int numOfEnemy = (pLevel - mod) / i;
            pLevel -= numOfEnemy * i;

            // gets a random enemy from list of enemy of level i
            // and adds to random roaster
            for (int b = 0; b < numOfEnemy; b++)
            { 
                int rand = Random.Range(0, list.Count);

                randomRoaster.Add(list[rand]);
                print(""+ numOfEnemy + " " + list[rand].name + " were added to Roaster ");

            }

            i--;
        }

        // Gets rid of enemy randomly from the randomRoaster
        // until there is not more than the limit
        int removeCount = Mathf.Max(0, randomRoaster.Count - possibleSpawnSpots.Count);
        for (int f = 0; f < removeCount; f++)
        {
            int rand = Random.Range(0, randomRoaster.Count);
            randomRoaster.RemoveAt(rand);
        }



    }

    private void startRandomCombat()
    {
        // Make a copy of spawn spots we can pull from
        List<GameObject> availableSpots = new List<GameObject>(possibleSpawnSpots);

        for (int f = 0; f < randomRoaster.Count; f++)
        {
            GameObject enemy = Instantiate(randomRoaster[f], RandomLevelObj.transform);


            int rand = Random.Range(0, availableSpots.Count);
            enemy.transform.position = availableSpots[rand].transform.position;

            // Remove used spot so it's not reused
            availableSpots.RemoveAt(rand);
        }
    }



    // sorts enemy levels from 1-7, else put them in level 999
    private void sortEnemyLists()
    {
        foreach (GameObject enemy in allEnemyPrefabList)
        {
            Entity e = enemy.GetComponent<Entity>();
            switch (e.fableWorth)
            {
                case 1:
                    enemyL1.Add(enemy);
                    break;
                case 2:
                    enemyL2.Add(enemy);
                    break;
                case 3:
                    enemyL3.Add(enemy);
                    break;
                case 4:
                    enemyL4.Add(enemy);
                    break;
                case 5:
                    enemyL5.Add(enemy);
                    break;
                case 6:
                    enemyL6.Add(enemy);
                    break;
                default:
                    enemyL999.Add(enemy);
                    break;
            }

        }
    }


}
