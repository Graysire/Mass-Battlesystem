using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    [SerializeField]
    protected string formationName;

    //the template the characters in this unit are based off of
    //will likely by removed once other battle set-up functionsare implemented
    [SerializeField]
    protected CharacterTemplate template;
    //the character whose stats are used for all memebrs ofthe unit
    [SerializeField]
    protected Character troop;
    //the width of each rank(row) of characters
    [SerializeField]
    protected int frontage;
    //the number of ranks(rows) of characters
    [SerializeField]
    protected int ranks;
    //the number of Characters remaining in this formation
    [SerializeField]
    protected int currentTroops;

    //targetted formation, used as a placeholder for when targeting controls are implemented
    public Formation target;

    //which direction this formation is facing, with 0 and going lcockwise, thus 5 being up-left
    [SerializeField]
    [Range(0, 5)]
    protected int facing;

    // Start is called before the first frame update
    void Start()
    {
        troop = new Character(template);
        currentTroops = frontage * ranks;
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // characters[0, 0].MeleeAttack(characters[1, 1]);
        if (Input.GetKeyDown(KeyCode.Space) && target != null)
        {
            string temp = target.formationName + ": " + target.currentTroops;
            this.MeleeAttack(target);
            temp += " -> " + target.currentTroops + " troops";
            Debug.Log(temp);
        }
        
    }

    public void MeleeAttack(Formation target)
    {
        //the number of attacks this formation will make
        int numAttackers;

        int targetRemainingFrontage = Mathf.Min(target.frontage, target.currentTroops);

        //if the target is wider or equal width, make attacks equal to frontage
        if (targetRemainingFrontage >= frontage)
        {
            numAttackers = frontage;
        }
        //if this formation is wider by more than 1, make attacks equal to frontage + 4
        else if (frontage > targetRemainingFrontage + 1)
        {
            numAttackers = targetRemainingFrontage + 2;
        }
        //if this formation is wider by 1, make attacks equal to frontage + 2
        else
        {
            numAttackers = targetRemainingFrontage + 1;
        }

        //if there are more attacks than troops remaining, reduce number of attacks to troops remaining
        if (numAttackers > currentTroops)
        {
            numAttackers = currentTroops;
        }



        //calculate what kind of facing is being attacked, front, front-flank, rear, or rear-flank
        int facingType = target.facing - facing;
        int facingType2 = facing - target.facing;
        //adjust the negative number(if one exists) to be positive
        if (facingType < 0)
        {
            facingType += 6;
        }
        else if (facingType2 < 0)
        {
            facingType2 += 6;
        }
        //calculate the shortest difference between the attacker's facing and the target's facing
        facingType = Mathf.Min(facingType, facingType2);
        //if facingType = 0, their facing is the same, so it's a rear attack
        //if facingType = 1, it's a rear-flank
        //if facingType = 2, it's a front flank
        //if facingType = 3, they are directly opposite each other so it's a frontal attack

        int bonusAttack = 0;

        //apply bonuses based on facing
        //flanking attacks on a hex grid result in less troops being able to attack, but gaining a bonus to hit
        if (facingType == 2) //front-flank
        {
            numAttackers = (int) Mathf.Round(0.75f * numAttackers);
            bonusAttack = 2;
            Debug.Log("Front-Flank Attack");
        }
        else if (facingType == 1) //rear-flank
        {
            numAttackers = (int)Mathf.Round(0.75f * numAttackers);
            bonusAttack = 4;
            Debug.Log("Rear-Flank Attack");
        }
        else if (facingType == 0) //rear
        {
            bonusAttack = 6;
            Debug.Log("Rear Attack");
        }




        //for each attack, attack, check if a character has died and apply the resulting effects
        for (int attacks = 0; attacks < numAttackers; attacks++)
        {
            troop.MeleeAttack(target.troop, bonusAttack);
            if (!target.troop.IsAlive())
            {
                target.troop.ResetHealth();
                target.currentTroops--;
                //if the remaining number of troops is less than 0, the formation is destroyed
                if (target.currentTroops <= 0)
                {
                    //unimplemented destroy formation
                }
                //if the remaining number of troops is less than ranks * frontage then the target formation has lost a rank
                else if (target.currentTroops <= ((target.ranks -1) * target.frontage))
                {
                    target.ranks--;
                }
            }
        }


        //deprecated version involving Characters being stored in an array, system was deemed unecessarily complex and never finished
        //determine how many characters can attack
        //int numAttackers = frontage;
        //if (frontage > target.frontage + 1)
        //{
        //    numAttackers += 2;
        //}

        //for (int attacker = 0; attacker < frontage; attacker++)
        //{
        //    for (int rank = 0; rank < ranks; rank++)
        //    {
        //        if (characters[rank, attacker].IsAlive())
        //        {
        //            for (int targetRank = 0; targetRank < target.ranks; rank++)
        //            {
        //            }
        //        }
        //    }
        //}
    }
}
