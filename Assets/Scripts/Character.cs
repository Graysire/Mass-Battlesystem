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

    //an attack from this character against a target character using a weapon
    public void Attack(Character target, Weapon weapon = null, int attackBonus = 0)
    {
        //rolls 1d20 before modifiers
        int dieRoll = Random.Range(1, 21);
        //bool stating if a natural 20 was rolled
        bool natTwenty = false;
        //a roll of 1 is always a fail
        if (dieRoll == 1)
        {
            return;
        }
        else if (dieRoll == 20)
        {
            natTwenty = true;
        }

        //if a 20 was not rolled, add modifiers, a 20 is an automatic success
        if (!natTwenty)
        {
            dieRoll += toHitBonus + attackBonus;
            //if the character is using a weapon and it is finesse, add dexterity
            if (weapon != null && weapon.GetIsFinesse() == true)
            {
                dieRoll += dexterity;
            }
            //otherwise use strength
            else
            {
                dieRoll += strength;
            }
        }

        //check if the attack hit the target
        if (natTwenty || dieRoll >= target.defence)
        {
            //declare damage roll
            int damageRoll;
            //if the weapon is finesse add dexterity to the damage
            if (weapon != null && weapon.GetIsFinesse() == true)
            {
                damageRoll = dexterity;
            }
            //else add 1.5x strength
            else
            {
                damageRoll = (int) (strength * 1.5);
            }

            //add damage rolls if the character is using a weapon
            if (weapon != null)
            {
                for (int i = 0; i < weapon.GetNumDice(); i++)
                {
                    damageRoll += Random.Range(1, weapon.GetDiceSize() + 1);
                }
            }
            else
            {
                //default damage is 1d2
                damageRoll += Random.Range(1, 3);
            }

            //reduce the target's health by the damage roll
            target.currentHealth -= damageRoll;
        }
    }

    //public void MeleeAttack(Character target, int attackBonus = 0)
    //{
    //    //string debug = "";

    //    int dieRoll = Random.Range(1, 21);
    //    //debug += "Rolled " + dieRoll + "(d20)+";
    //    dieRoll += strength + toHitBonus + attackBonus;
    //    //debug += (strength + toHitBonus) + " vs " + target.defence;

    //    if (dieRoll >= target.defence)
    //    {
    //        //debug += " Hitting, dealing ";
    //        int damageRoll = Random.Range(1, 9);
    //        //debug += damageRoll + "(d8)+";
    //        damageRoll += (int) (strength * 1.5);
    //        //debug += ((int)(strength * 1.5)) + " damage";
    //        target.currentHealth -= damageRoll;
    //    }

    //    //Debug.Log(debug);
    //}

    //public void RangedAttack(Character target, int attackBonus = 0)
    //{
    //    //string debug = "";

    //    int dieRoll = Random.Range(1, 21);
    //    //debug += "Rolled " + dieRoll + "(d20)+";
    //    dieRoll += dexterity + toHitBonus + attackBonus;
    //    //debug += (strength + toHitBonus) + " vs " + target.defence;

    //    if (dieRoll >= target.defence)
    //    {
    //        //debug += " Hitting, dealing ";
    //        int damageRoll = Random.Range(1, 9);
    //        //debug += damageRoll + "(d8)";
 
    //        target.currentHealth -= damageRoll;
    //    }

    //    //Debug.Log(debug);
    //}

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
        //currentHealth = maxHealth;
        maxHealth = 0;
        for (int i = 0; i < numHitDice; i++)
        {
            maxHealth += Random.Range(1, hitDiceSize + 1) + constitution;
        }
        currentHealth = maxHealth;
    }
}
