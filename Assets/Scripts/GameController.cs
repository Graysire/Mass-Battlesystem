using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //the current phase of the battle
    public static BattlePhase currentPhase;
    //the main GameController
    public static GameController main;
    //the lsit of all Formations participating in the battle
    public List<Army> armyList;

    private void Awake()
    {
        //the first GameControll should become the main one
        if (main == null)
        {
            main = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = BattlePhase.MISSILE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //changes to the next phase of the game
    public void ChangePhase()
    {
        switch (currentPhase)
        {
            case BattlePhase.MISSILE:
                //Casualty sub-phase
                foreach (Army army in armyList)
                {
                    army.EndCombatPhase();
                }
                currentPhase = BattlePhase.MOVEMENT;
                break;
            case BattlePhase.MOVEMENT:
                foreach (Army army in armyList)
                {
                    army.EndMovementPhase();
                }
                currentPhase = BattlePhase.MELEE;
                break;
            case BattlePhase.MELEE:
                foreach (Army army in armyList)
                {
                    army.EndCombatPhase();
                }
                currentPhase = BattlePhase.MORALE;
                break;
            case BattlePhase.MORALE:
                foreach (Army army in armyList)
                {
                    army.EndMoralePhase();
                }
                currentPhase = BattlePhase.MISSILE;
                break;
        }
        Debug.Log("Current Phase is now " + currentPhase);
    }

    public enum BattlePhase
    {
        MISSILE, MOVEMENT, MELEE, MORALE
    }
}
