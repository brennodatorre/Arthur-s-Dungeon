using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class DiceRoll 
{
    [SerializeField] public List<Dice> dices = new List<Dice>();
    [SerializeField] public int modifier = 0;

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
    public void AddDice(DiceRoll _dices)
    {
        foreach (var die in _dices.dices)
        {
            dices.Add(new Dice(die.count, die.sides));
        }
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
    public void RemoveDice(DiceRoll _dice)
    {
        foreach (var die in _dice.dices)
        {
            RemoveDice(die.count, die.sides, _dice);
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
    
    //rolls the dices and return the result + the modifier
    public int Roll(int advantages = 0)
    {
        

        int rolls = Mathf.Abs(advantages) + 1;

        int bestTotal = 0;
        bool firstRoll = true;

        for (int i = 0; i < rolls; i++)
        {
            int total = 0;

            foreach (var dice in dices)
            {
                if (dice.count <= 0 || dice.sides <= 0)
                    continue;

                for (int j = 0; j < dice.count; j++)
                {
                    total += Random.Range(1, dice.sides + 1);
                }
            }

            if (firstRoll)
            {
                bestTotal = total;
                firstRoll = false;
            }
            else if (advantages >= 0) // advantage
            {
                bestTotal = Mathf.Max(bestTotal, total);
            }
            else // disadvantage
            {
                bestTotal = Mathf.Min(bestTotal, total);
            }
        }

        return bestTotal + modifier;

        
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

    /// <summary>
    /// Rolls a Trait test based on the Trait Scaling
    /// </summary>
    /// <param name="level"></param>
    /// <returns> The result of the roll </returns>
    public static int rollTest (int level)
    {
        int tier = level / 3;
        int modifier = tier * 5;

        int advantage = (level % 3) + 1;

        DiceRoll roll = new DiceRoll(new List<Dice> { new Dice(1, 20) }, modifier);

        return roll.Roll(advantage);
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
