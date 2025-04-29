using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[System.Serializable]
public class DiceRoll 
{
    [SerializeField] public List<Dice> dices = new() ;
    [SerializeField] private int modifier = 0;

    public DiceRoll() { }

    public DiceRoll(List<Dice> dices, int modifier = 0)
    {
        this.dices = dices;
        this.modifier = modifier;
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
    public DiceRoll AddModifier(int value)
    {
        modifier += value;
        return this;
    }

    //rolls the complete dice pool and returns the total value
    public int Roll()
    {
        int total = 0;
        foreach (var dice in dices)
        {
            for (int i = 0; i < dice.count; i++)
            {
                int temp = Random.Range(1, dice.sides + 1);
                total += temp;
                //Debug.Log($"Rolled {temp} on {dice.count}d{dice.sides}");
            }
        }
        return total + modifier;
    }

    public int RollWithAdvantage(int advantages)
    {
        int finaltotal = 0;
        int total;
        for (int i = 0; i < advantages; i++)
        {
            total = 0;
            foreach (var dice in dices)
            {
                for (int j = 0; j < dice.count; j++)
                {
                    total += Random.Range(1, dice.sides + 1);
                }
            }
            if (total > finaltotal) finaltotal = total;
        }
        return finaltotal + modifier;

        
    }

      public override string ToString()
    {
        List<string> parts = new();
        foreach (var dice in dices)
        {
            parts.Add($"{dice.count}d{dice.sides}");
        }

        if (modifier != 0)
            parts.Add($"{(modifier > 0 ? "+" : "")}{modifier}");

        return string.Join(" + ", parts);
    }
}
