using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manages player input controls
public class PlayerController : MonoBehaviour
{

    //the formation that the player has selected
    //as functionality is added this may need to berepalced iwth a more generic 'Selectable' interface
    [SerializeField]
    Formation selectedFormation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if the player left clicks
        if (Input.GetMouseButtonDown(0))
        {
            //calculate whether anything has been clicked on
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            //if an object containing a formation was clicked on, select it
            if (hit)
            {
                selectedFormation = hit.collider.GetComponent<Formation>();
                Debug.Log(selectedFormation.GetName());
            }
        }
        //if the player right clicks
        if (Input.GetMouseButtonDown(1))
        {
            
            //calculate whether anything has been clicked on
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            Formation temp = null;

            if (hit)
            {
                temp = hit.collider.GetComponent<Formation>();
                //Debug.Log(selectedFormation.GetName());
            }

            //take an action based on the current phase
            switch (GameController.currentPhase)
            {
                case GameController.BattlePhase.MISSILE:
                    if (temp != null)
                    {
                        //add prompt or seperate controls for non-volley missile fire
                        selectedFormation.RangedAttack(temp, true);
                    }
                    break;
                case GameController.BattlePhase.MOVEMENT:
                    StartCoroutine(selectedFormation.MoveToHex(mousePos));
                    break;
                case GameController.BattlePhase.MELEE:
                    if (temp != null)
                    {
                        selectedFormation.MeleeAttack(temp);
                    }
                    break;
            }
        }
        //if hitting enter, change the phase
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameController.main.changePhase();
        }
        //turn to the right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedFormation != null)
            {
                //right arrow key turns the selectedformation to the right
                selectedFormation.changeFacing(1);
            }
        }
        //turn to the left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedFormation != null)
            {
                //right arrow key turns the selectedformation to the right
                selectedFormation.changeFacing(-1);
            }
        }
    }
}
