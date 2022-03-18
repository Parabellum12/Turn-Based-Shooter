using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_Unit_Handler_Script : MonoBehaviour
{
    GameObject gameHadlerObj;
    Game_Handler gameHandlerScript;
    public CharacterData characterData;
    public Vector2Int gridPos;
    float speed = 50f;
    private void Start()
    {
        gameHadlerObj = GameObject.FindGameObjectWithTag("GameController");
        gameHandlerScript = gameHadlerObj.GetComponent<Game_Handler>();
    }

    public void setup(Vector3 pos, CharacterData charDat, Vector2Int gridpos)
    {
        characterData = charDat;
        transform.position = pos;
        this.gridPos = gridpos;
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

    public IEnumerator moveToPos(Vector2Int[] posList)
    {
        Debug.Log("Move");
        foreach (Vector2Int vec in posList)
        {
            targetPos = gameHandlerScript.getPosOnGrid(vec);
            needToMove = true;
            while (Vector2.Distance(transform.position, targetPos) > 0.5f)
            {
                yield return null;
            }
            transform.position = targetPos;
        }
        needToMove = false;
        seGridPos(posList[posList.Length-1]);
        yield break;
    }
    bool needToMove = false;
    Vector2 targetPos;

    private void Update()
    {
        if (needToMove)
        {
            transform.position = new Vector2(HandleMoveSingleAxis(transform.position.x, targetPos.x, speed), HandleMoveSingleAxis(transform.position.y, targetPos.y, speed));
        }
    }

    private float HandleMoveSingleAxis(float current, float target, float speed)
    {
        if (Mathf.Abs(Mathf.Abs(current) - Mathf.Abs(target)) < 0.1f)
        {
            return target;
        }
        if (current < target)
        {
            //move forward
            return current + (speed * Time.deltaTime);
        }
        else
        {
            //move back
            return current - (speed * Time.deltaTime);
        }
    }



    public void seGridPos(Vector2Int pos)
    {
        gridPos = pos;
    }

    public Vector2Int getGridPos()
    {
        return gridPos;
    }
}
