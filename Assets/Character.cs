using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character : CharacterTemplate
{
    //the number of additional hit dice this character has gained
    [SerializeField]
    protected int level;

    //the bonus to hit this character has due to skill
    protected int toHitBonus;
    //the bonus to defence this character has due to skill
    protected int defenceBonus;
    //the difficulty of hitting this character
    [SerializeField]
    protected int defence;
    //the health this character has remaining
    [SerializeField]
    protected int currentHealth;
    //the maximum health this character can have
    [SerializeField]
    protected int maxHealth;

    //instantiates a character based off a template
    public Character(CharacterTemplate template) : base(template)
    {
        level = 0;
        toHitBonus = (int) (numHitDice * toHitProgression);
        defenceBonus = (int)(numHitDice * defenceProgression);
        defence = 12 + defenceBonus + dexterity;
        maxHealth = 0;
        for (int i = 0; i < numHitDice; i++)
        {
            maxHealth += Random.Range(1, hitDiceSize + 1) + constitution;
        }
        currentHealth = maxHealth;
    }

    public override string ToString()
    {
        string temp = "";
        temp += currentHealth + "/" + maxHealth;
        return temp;
    }

    public void MeleeAttack(Character target)
    {
        //string debug = "";

        int dieRoll = Random.Range(1, 21);
        //debug += "Rolled " + dieRoll + "(d20)+";
        dieRoll += strength + toHitBonus;
        //debug += (strength + toHitBonus) + " vs " + target.defence;

        if (dieRoll >= target.defence)
        {
            //debug += " Hitting, dealing ";
            int damageRoll = Random.Range(1, 9);
            //debug += damageRoll + "(d8)+";
            damageRoll += (int) (strength * 1.5);
            //debug += ((int)(strength * 1.5)) + " damage";
            target.currentHealth -= damageRoll;
        }

        //Debug.Log(debug);
    }

    //returns if this character's current health is less than 0 or not
    public bool IsAlive()
    {
        if (currentHealth <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //resets the current health to equal the maximum health
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
