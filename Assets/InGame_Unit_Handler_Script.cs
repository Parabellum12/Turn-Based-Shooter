using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_Unit_Handler_Script : MonoBehaviour
{
    GameObject gameHadlerObj;
    Game_Handler gameHandlerScript;
    public CharacterData characterData;

    private void Start()
    {
        gameHadlerObj = GameObject.FindGameObjectWithTag("GameController");
        gameHandlerScript = gameHadlerObj.GetComponent<Game_Handler>();
    }

    public void setup(Vector3 pos, CharacterData charDat)
    {
        characterData = charDat;
        transform.position = pos;
    }


    public bool mouseOver = false;

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver=false;
    }

    public void moveToPos(Vector2Int[] posList)
    {
        Debug.Log("Move");
        foreach (Vector2Int vec in posList)
        {
            transform.position = gameHandlerScript.getPosOnGrid(vec);
        }
    }

}
