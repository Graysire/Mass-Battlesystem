using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static BattlePhase currentPhase;

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = BattlePhase.MOVEMENT;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum BattlePhase
    {
        MISSILE, CASUALTY, MOVEMENT, MELEE, MORALE
    }
}
