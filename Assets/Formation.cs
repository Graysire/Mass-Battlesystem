using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    //the template the characters in this unit are based off of
    [SerializeField]
    protected CharacterTemplate template;
    //the width of each rank(row) of characters
    [SerializeField]
    protected int frontage;
    //the number of ranks(rows) of characters
    [SerializeField]
    protected int ranks;

    //the matrix representing the characters making up this formation
    protected Character[,] characters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        characters = new Character[ranks, frontage];
        for (int rank = 0; rank < ranks; rank++)
        {
            for (int column = 0; column < frontage; column++)
            {
                characters[rank, column] = new Character(template);
                Debug.Log(characters[rank, column]);
            }
        }
        //Debug.Log(characters[0, 0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
