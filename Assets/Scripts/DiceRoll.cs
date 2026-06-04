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


    /// <summary>
    /// Creates a new DiceRoll that is a true copy of the given DiceRoll
    /// </summary>
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
        for (int i = 0; i < dices.Count; i++)
        {
            if (dices[i].sides == sides)
            {
                Dice dice = dices[i];
                dice.count += count;
                if (dice.count == 0)
                {
                    dices.RemoveAt(i);
                }
                else
                {
                    dices[i] = dice;
                }



                return this;
            }
        }

        dices.Add(new Dice(count, sides));
        return this;
    }
    public void AddDice(DiceRoll _dices)
    {
        if (_dices == null) return;
        foreach (var die in _dices.dices)
        {
            AddDice(die.count, die.sides);
        }
    }

    public void RemoveDice(int count, int sides)
    {
        for (int i = 0; i < dices.Count; i++)
        {
            if (dices[i].sides == sides)
            {
                Dice dice = dices[i];
                int temp = dice.count - count;
                if (temp > 0)
                {
                    dice.count = temp;
                    dices[i] = dice;
                    
                }
                else if (temp == 0)
                {
                    dices.RemoveAt(i);
                    
                }
                else
                {
                    dices.RemoveAt(i);
                    AddDice(temp, sides);
                }

                return;
  
            }
        }

        //add a negative dice if list doesnt have one to removefom

       dices.Add(new Dice(-count, sides));
        
    }
    public void RemoveDice(DiceRoll _dice)
    {
        foreach (var die in _dice.dices)
        {
            RemoveDice(die.count, die.sides);
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

    
    //rolls the dices and return the result + the modifier
    public int Roll(int advantages = 0)
    {
        

        return RollWithCritCheck(advantages).value;
        
    }


    /// <summary>
    /// Returns the result of the roll, as well as whether it was a critical hit or critical fail
    /// </summary>
    public (int value, bool critHit, bool critFail) RollWithCritCheck(int advantages = 0)
    {
        int rolls = Mathf.Abs(advantages) + 1;

        int bestTotal = 0;
        bool firstRoll = true;

        
        int bestPositiveRolls = 0;

        for (int i = 0; i < rolls; i++)
        {
            int total = 0;
            int positiveRolls = 0;
            foreach (var dice in dices)
            {
                if (dice.count <= 0) { //negative dice, remove from total
                    
                    for (int j = 0; j < Mathf.Abs(dice.count); j++)
                    {
                        total -= Random.Range(1, dice.sides + 1);
                    }
                    continue;
                }


                for (int j = 0; j < dice.count; j++)
                {
                    int roll = Random.Range(1, dice.sides + 1);
                    total += roll;
                    positiveRolls += roll;
                }
            }

            if (firstRoll)
            {
                bestTotal = total;
                bestPositiveRolls = positiveRolls;
                firstRoll = false;
            }
            else if (advantages >= 0) // advantage
            {
                if (total > bestTotal)
                {
                    bestTotal = total;
                    bestPositiveRolls = positiveRolls;
                }
            }
            else // disadvantage
            {
                if (total < bestTotal)
                {
                    bestTotal = total;
                    bestPositiveRolls = positiveRolls;
                }
            }
        }

        Debug.Log("Rolled a total of " + bestTotal + " before modifier." + " Modifier is " + modifier + ". Final result is " + (bestTotal + modifier) + ".");


        return (bestTotal + modifier, wasCriticalHit(bestPositiveRolls), wasCriticalFail(bestPositiveRolls));
    }

    /// <summary>
    /// Checks crit hit with modifier
    /// </summary>
    private bool wasCriticalHit(int value){

        int maxRoll = 0;

        foreach (var dice in this.dices)
        {
            if (dice.count > 0) maxRoll += dice.sides * dice.count;
        }

        return value == maxRoll ;


    }
    /// <summary>
    /// Checks crit fail with modifier
    /// </summary>
    private bool wasCriticalFail(int value){

        int minRoll = 0;

        foreach (var dice in this.dices)
        {
            if (dice.count > 0) minRoll +=  dice.count;
        }

        return value == minRoll ;


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

        int advantage = (level % 3) ;

        DiceRoll roll = new DiceRoll(new List<Dice> { new Dice(1, 20) }, modifier);

        return roll.Roll(advantage);
    }

    
    override public string ToString()
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


    public DiceRoll Clone()
    {
        return new DiceRoll(this);
    }
}
