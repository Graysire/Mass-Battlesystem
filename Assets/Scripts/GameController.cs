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
    public List<Formation> formationList;

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
        currentPhase = BattlePhase.MOVEMENT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //changes to the next phase of the game
    public void changePhase()
    {
        switch (currentPhase)
        {
            case BattlePhase.MISSILE:
                //Casualty sub-phase
                foreach (Formation form in formationList)
                {
                    form.ApplyCasualties();
                    form.ResetMovement();
                }
                currentPhase = BattlePhase.MOVEMENT;
                break;
            case BattlePhase.MOVEMENT:
                currentPhase = BattlePhase.MELEE;
                break;
            case BattlePhase.MELEE:
                foreach (Formation form in formationList)
                {
                    form.ApplyCasualties();
                }
                currentPhase = BattlePhase.MORALE;
                break;
            case BattlePhase.MORALE:
                currentPhase = BattlePhase.MISSILE;
                break;
        }
    }

    public enum BattlePhase
    {
        MISSILE, MOVEMENT, MELEE, MORALE
    }
}
