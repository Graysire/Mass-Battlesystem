using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//represents a weapon wielded by a character
public class Weapon
{
    //the name of the weapon
    [SerializeField]
    string wepName;

    //how far in hexes this weapon can reach
    [SerializeField]
    int range;
    //how many ranks can attack in melee
    [SerializeField]
    int reach;
    //number of damage dice
    [SerializeField]
    int numDice;
    //size of damage dice
    [SerializeField]
    int diceSize;

    //whether the weapon uses DEX or STR for its bonus to hit and damage
    [SerializeField]
    bool isFinesse;
    //whether the weaponcan be used to attack with all mebers of the unit at a penalty
    [SerializeField]
    bool canVolley;

    public Weapon(string name, int range, int reach, int numDice, int diceSize, bool isFinesse = false, bool canVolley = false)
    {
        wepName = name;
        this.range = range;
        this.reach = reach;
        this.numDice = numDice;
        this.diceSize = diceSize;
        this.isFinesse = isFinesse;
        this.canVolley = canVolley;
    }

    //returns the weapon's name
    public string GetName()
    {
        return wepName;
    }

    //returns the weapon's range
    public int GetRange()
    {
        return range;
    }

    //returns the weapon's reach
    public int GetReach()
    {
        return reach;
    }

    //returns the weapon's number of damage dice
    public int GetNumDice()
    {
        return numDice;
    }

    //returns the weapon's damage dice size
    public int GetDiceSize()
    {
        return diceSize;
    }

    //returns whether the weapon is a finesse weapon
    public bool GetIsFinesse()
    {
        return isFinesse;
    }

    //returns whether the weapon is a finesse weapon
    public bool GetCanVolley()
    {
        return canVolley;
    }
}
