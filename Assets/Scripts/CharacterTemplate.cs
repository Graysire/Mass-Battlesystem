using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterTemplate
{
    //number of hit dice rolled to determine hit points
    [SerializeField]
    protected int numHitDice;
    //the size of the hit dice rolled to determine hit points
    [SerializeField]
    protected int hitDiceSize;

    //the bonus a character has to hit and damage in melee due to raw strength
    [SerializeField]
    protected int strength;
    //the bonus a character has to hit and damage at range due to raw finesse
    [SerializeField]
    protected int dexterity;
    //the bonus a character has to hit points per hit dice
    [SerializeField]
    protected int constitution;
    //how fast the character can move across the battlefield
    [SerializeField]
    protected int speed;
    //how likely this character is to run away
    [SerializeField]
    protected int morale;

    //the bonus a character gains to hit per hit dice
    [SerializeField]
    protected float toHitProgression;
    //the bonus a character gains to defence per hit dice
    [SerializeField]
    protected float defenceProgression;

    public CharacterTemplate(CharacterTemplate template)
    {
        numHitDice = template.numHitDice;
        hitDiceSize = template.hitDiceSize;
        strength = template.strength;
        dexterity = template.dexterity;
        constitution = template.constitution;
        speed = template.speed;
        morale = template.morale;
        toHitProgression = template.toHitProgression;
        defenceProgression = template.defenceProgression;
    }

    //returns the speed of the character
    public int GetSpeed()
    {
        return speed;
    }
}
