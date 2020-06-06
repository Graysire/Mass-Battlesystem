﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    //the name of this Formation
    [SerializeField]
    protected string formationName;
    //the pathgrid used for this Formation's pathfinding
    PathGrid pathGrid;

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

    //the delay between movements when animating movement between hexes
    [SerializeField]
    float movementDelay;
    //boolean to skip the next animation
    bool skipNextAnimation;
    //boolean to determine if an animation is ongoing (and prevent playing other animations while true)
    //this should only be used in cases where rapid animation could cause issues with the game state, such as rapid movement causing inconsistencies in tile obstruction levels
    bool isAnimating;

    // Start is called before the first frame update
    void Start()
    {
        troop = new Character(template);
        currentTroops = frontage * ranks;

        //get the rotation of the current object
        Quaternion rotation = gameObject.transform.rotation;

        //calculate the facing from the rotation
        facing = ((int) Mathf.Round((360 - transform.rotation.eulerAngles.z) / 60)) % 6;
        //set the rotation based on the facing to ensure it's correct
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, facing * -60);

        //sets the position of this Formation to be the center of its hex
        transform.position = pathGrid.NodeToWorld(pathGrid.WorldToNode(transform.position));

        //sets the hex this Formation occupies to be move obstructed
        pathGrid.WorldToNode(transform.position).isMoveObstructed = true;
    }

    private void Awake()
    {
        //gets the scene's pathgrid
        pathGrid = GameObject.Find("PathManager").GetComponent<PathGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        // characters[0, 0].MeleeAttack(characters[1, 1]);
        if (Input.GetKeyDown(KeyCode.Alpha1) && target != null)
        {
            string temp = target.formationName + ": " + target.currentTroops;
            this.MeleeAttack(target);
            temp += " -> " + target.currentTroops + " troops";
            Debug.Log(temp);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && target != null)
        {
            string temp = target.formationName + ": " + target.currentTroops;
            this.RangedAttack(target, false);
            temp += " -> " + target.currentTroops + " troops";
            Debug.Log(temp);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && target != null)
        {
            string temp = target.formationName + ": " + target.currentTroops;
            this.RangedAttack(target, true);
            temp += " -> " + target.currentTroops + " troops";
            Debug.Log(temp);
        }
        else if (target == this && Input.GetKeyDown(KeyCode.Alpha4) && !isAnimating)
        {
            StartCoroutine(MoveToHex(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            skipNextAnimation = true;
        }

    }

    //this formation makes a number of attacks against the tagret formation, possibly damaging it
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
        int facingType = Mathf.Min(Mathf.Abs(target.facing - facing), 6 - Mathf.Abs(target.facing - facing));

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

    public void RangedAttack(Formation target, bool isVolley)
    {
        //the number of attacks this formation will make
        int numAttackers;
        //the bonus or penalty to hit for the attacks made
        int bonusAttack = 0;

        //if the formation is volley firing, all of its troops can attack with a penalty
        if (isVolley)
        {
            numAttackers = currentTroops;
            bonusAttack = -4;
        }
        //for direct fire, only the first two ranks can attack
        else
        {
            numAttackers = Mathf.Min(2 * frontage, currentTroops);
        }

        //for each attack, attack, check if a character has died and apply the resulting effects
        for (int attacks = 0; attacks < numAttackers; attacks++)
        {
            troop.RangedAttack(target.troop, bonusAttack);
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
                else if (target.currentTroops <= ((target.ranks - 1) * target.frontage))
                {
                    target.ranks--;
                }
            }
        }
    }

    IEnumerator MoveToHex(Vector3 targetLocation)
    {
        //lock animation
        isAnimating = true;

        //find a path using the pathgrid
        pathGrid.getFinalPath(transform.position, targetLocation, troop.GetSpeed(), facing);
        //if a path exists, move to it
        if (pathGrid.finalPath.Count != 0)
        {
            //store the final node in case the coroutine needs to end early
            PathNode finalNode = pathGrid.finalPath[pathGrid.finalPath.Count - 1];
            //go through each point
            for (int i = 1; i < pathGrid.finalPath.Count; i++)
            {
                //if the animation is not being skipped
                if (!skipNextAnimation)
                {
                    //move to next point, set facing, and wait for the movement delay
                    transform.position = pathGrid.NodeToWorld(pathGrid.finalPath[i]);
                    facing = pathGrid.finalPath[i].prevFacing;

                    //set the rotation based on the facing
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, facing * -60);
                    yield return new WaitForSeconds(movementDelay);
                }
                else
                {
                    //if the animation is being skipped go to the final node and break
                    transform.position = pathGrid.NodeToWorld(finalNode);
                    facing = finalNode.prevFacing;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, facing * -60);
                    skipNextAnimation = false;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("No path exists to the target point or the point is out of range");
        }
        //unlock animation
        isAnimating = false;
    }

}
