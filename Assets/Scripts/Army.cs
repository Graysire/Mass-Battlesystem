using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army : MonoBehaviour
{
    //the name of this army
    [SerializeField]
    protected string armyName;

    //the list of all Formations from this army participating in the battle
    protected List<Formation> formationList;
    //the total number of troops in this army
    int armyTroops = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameController.main.armyList.Add(this);
        //get initial troop number
        foreach (Formation form in formationList)
        {
            armyTroops += form.GetCurrentTroops();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //adds a formation to the army's list of formations
    public void AddFormation(Formation formation)
    {
        formationList.Add(formation);
        armyTroops += formation.GetCurrentTroops();
    }

    //handles the end of the missile and melee phases
    public void EndCombatPhase()
    {
        //the new total number of troops
        int newTroopNumber = 0;

        //reset whether each formation has attacked and apply any casualties
        foreach (Formation form in formationList)
        {
            newTroopNumber += form.ApplyCasualties();
            form.SetHasAttacked(false);
        }

        armyTroops = newTroopNumber;
    }

    //handles the end of the movement phase
    public void EndMovementPhase()
    {
        //reset the movement of all formations
        foreach (Formation form in formationList)
        {
            form.ResetMovement();
        }
    }

    //handles the end of the morale phase
    public void EndMoralePhase()
    {
        //the new total number of troops
        int newTroopNumber = 0;

        //apply the effects of morale to all formations
        foreach (Formation form in formationList)
        {
            newTroopNumber += form.ApplyMorale();
        }

        armyTroops = newTroopNumber;
    }
}
