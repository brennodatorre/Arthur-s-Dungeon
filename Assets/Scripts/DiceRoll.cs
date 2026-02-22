using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class DiceRoll 
{
    [SerializeField] public List<Dice> dices = new List<Dice>();
    [SerializeField] private int modifier = 0;

    public DiceRoll() { }

    public DiceRoll(List<Dice> dices, int modifier = 0)
    {
        this.dices = dices;
        this.modifier = modifier;
    }

    public DiceRoll(DiceRoll dc)
    {
        this.dices = new List<Dice>();
        foreach (var die in dc.dices)
        {
            this.dices.Add(new Dice (die.count, die.sides)); 
        }
        this.modifier = dc.modifier;
    }
    

    public DiceRoll AddDice(int count, int sides)
    {

        dices.Add(new Dice(count, sides));
        return this;
    }

    public void RemoveDice(int count, int sides, DiceRoll diceRoll)
    {
        foreach (var dice in dices)
        {
            if (dice.count == count && dice.sides == sides)
            {
                dices.Remove(dice);
                break;
            }
        }
        
    }

    //adds a modifier to the dice pool, this is added after rolling the dices
    public void AddModifier(int value)
    {
        this.modifier += value;
        
    }

    public int getModifier()
    {
        return this.modifier;
    }


    
    //internal helper method to check if the roll is safe to do
    private bool isSafeRoll(DiceRoll roll)
    {
        bool safe = true;

        int xSides = 0;
        int xDices = 0;
        foreach (Dice dice in roll.dices) { 
            xSides += dice.sides;
            xDices += dice.count;

            }

            if (xSides == xDices ) {safe = false;}

        return   safe;
    }
    
    //rolls the dices and return the result without the modifier
    public int Roll(int advantages = 0)
    {
        //returns random low negative num, showing its not a safe roll to do
        //if (!isSafeRoll(this)) { return -1 - Random.Range(999, 9999);}

        advantages++;

        int finaltotal = 0;
        int total;
        for (int i = 0; i < advantages; i++)
        {
            total = 0;
            foreach (var dice in dices)
            {
                for (int j = 0; j < dice.count; j++)
                {
                    if (dice.count <= 0 || dice.sides <= 0)
                    continue;

                    total += Random.Range(1, dice.sides + 1);
                }
            }
            if (total > finaltotal) finaltotal = total;
        }
        return finaltotal + modifier;

        
    }

    public bool wasCriticalHit(int value){

        int maxRoll = 0;

        foreach (var dice in this.dices)
        {
            maxRoll += dice.sides * dice.count;
        }

        return value == maxRoll;


    }
    public bool wasCriticalFail(int value){

        int minRoll = 0;

        foreach (var dice in this.dices)
        {
            minRoll +=  dice.count;
        }

        return value == minRoll;


    }


    


      public string diceToString()
    {
        List<string> parts = new();
        foreach (var dice in dices)
        {
            parts.Add($"{dice.count}d{dice.sides}");
        }

        if (modifier != 0)
            parts.Add($"{(modifier > 0 ? "+" : "")}{modifier}");

        return string.Join(" ", parts);
    }
}
