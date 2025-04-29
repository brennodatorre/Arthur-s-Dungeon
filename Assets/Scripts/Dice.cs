using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Dice
{
    public int count;
    public int sides;

    public Dice(int count, int sides)
    {
        this.count = count;
        this.sides = sides;
    }

    public override string ToString() => $"{count}d{sides}";
}
