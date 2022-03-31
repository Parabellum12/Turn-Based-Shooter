using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InGame_Unit_Handler_Script : MonoBehaviour
{
    GameObject gameHadlerObj;
    Game_Handler gameHandlerScript;
    public CharacterData characterData;
    public Vector2Int gridPos;
    float speed = 25f;
    [SerializeField] Sprite Attacker;
    [SerializeField] Sprite Defender;
    [SerializeField] Sprite Ranger;
    [SerializeField] Sprite Engineer;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer indicator;
    [SerializeField] PhotonView localview;
    [SerializeField] GameObject FOVObj;
    GameObject fovSystem;
    FieldOfView_Script localViewSystem;
    bool doneOnce = true;
    private void Start()
    {
        gameHadlerObj = GameObject.FindGameObjectWithTag("GameController");
        gameHandlerScript = gameHadlerObj.GetComponent<Game_Handler>();
        setuplocalViewSys();
    }

    void setuplocalViewSys()
    {
        if (!doneOnce)
        {
            return;
        }
        doneOnce = false;
        fovSystem = Instantiate(FOVObj);
        localViewSystem = fovSystem.GetComponent<FieldOfView_Script>();

        localViewSystem.lockOnTo = gameObject.transform;
    }



    public void setup(Vector3 pos, CharacterData charDat, Vector2Int gridpos)
    {
        setuplocalViewSys();
        characterData = charDat;
        int spritenum = 0;
        switch (characterData.characterClass)
        {
            case CharacterData.CharacterClassEnum.Attacker:
                sprite.sprite = Attacker;
                localViewSystem.setParameters(145, 45, 75);
                spritenum = 1;
                break;
            case CharacterData.CharacterClassEnum.Defender:
                sprite.sprite = Defender;
                localViewSystem.setParameters(280, 45, 35);
                spritenum = 2;
                break;
            case CharacterData.CharacterClassEnum.Ranger:
                sprite.sprite = Ranger;
                localViewSystem.setParameters(30, 60, 120);

                spritenum = 3;
                break;
            case CharacterData.CharacterClassEnum.Engineer:
                sprite.sprite = Engineer;
                localViewSystem.setParameters(360, 180, 25);
                spritenum = 4;
                break;

        }
        transform.position = new Vector3(pos.x, pos.y, -1);
        this.gridPos = gridpos;
        indicator.color = Color.green;

        localview.RPC("syncSprite", RpcTarget.Others, spritenum);
    }

    [PunRPC]
    void syncSprite(int spritenum)
    {
        switch (spritenum)
        {
            case 1:
                sprite.sprite = Attacker;
                break;
            case 2:
                sprite.sprite = Defender;
                break;
            case 3:
                sprite.sprite = Ranger;
                break;
            case 4:
                sprite.sprite = Engineer;
                break;
        }
    }



    public bool mouseOver = false;

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }


    //movment stuff
    bool wantToCancelMove = false;
    public IEnumerator moveToPos(Vector2Int[] posList)
    {
        Vector2 originalPos = transform.position;
        //Debug.Log("Move");
        foreach (Vector2Int vec in posList)
        {
            targetPos = gameHandlerScript.getPosOnGrid(vec);
            needToMove = true;
            setAngle(originalPos, targetPos);
            while (Vector2.Distance(transform.position, targetPos) > 0.5f)
            {
                yield return null;
            }
            if (wantToCancelMove)
            {
                wantToCancelMove = false;
                break;
            }
            transform.position = new Vector3(targetPos.x, targetPos.y, -1);
            originalPos = new Vector3(targetPos.x, targetPos.y, -1);
        }
        needToMove = false;
        seGridPos(posList[posList.Length - 1]);
        yield break;
    }

    public IEnumerator stopMovement()
    {
        yield return StartCoroutine(waitForReaction());
        wantToCancelMove = true;
        //Debug.Log("start stop movment");
        yield break;
    }

    float getTimeToReact()
    {
        return (1000 - (characterData.ReactionTime * 10)) / 1000;
    }

    IEnumerator waitForReaction()
    {
        float timeToReact = getTimeToReact();

        float startTime = Time.realtimeSinceStartup;
        //Debug.Log((Time.realtimeSinceStartup - startTime));
        while ((Time.realtimeSinceStartup - startTime) < timeToReact)
        {
            Debug.Log("reactionTime:" + timeToReact + " TimeSinceStart:" + (Time.realtimeSinceStartup - startTime) + " Test:" + ((Time.realtimeSinceStartup - startTime) < timeToReact));
            yield return null;
        }
        yield break;
    }

    private void setAngle(Vector2 original, Vector2 target)
    {
        if (original.x == target.x)
        {
            //vertical
            if (original.y < target.y)
            {
                //up
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                //down
                transform.rotation = Quaternion.Euler(0, 0, -180);
            }
        }
        else if (original.y == target.y)
        {
            //horizontal
            if (original.x < target.x)
            {
                //right
                transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                //left
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        else
        {
            //diagonal
            if (target.x < original.x && original.y < target.y)
            {
                //topleft
                transform.rotation = Quaternion.Euler(0, 0, 45);
            }
            else if (original.x < target.x && original.y < target.y)
            {
                //topright
                transform.rotation = Quaternion.Euler(0, 0, -45);
            }
            else if (original.x < target.x && original.y > target.y)
            {
                //bottomright
                transform.rotation = Quaternion.Euler(0, 0, -135);
            }
            else if (target.x < original.x && target.y < original.y)
            {
                //bottomleft
                transform.rotation = Quaternion.Euler(0, 0, 135);
            }
        }
    }

    public bool needToMove = false;
    Vector2 targetPos;

    private void Update()
    {
        if (needToMove)
        {
            transform.position = new Vector3(HandleMoveSingleAxis(transform.position.x, targetPos.x, speed), HandleMoveSingleAxis(transform.position.y, targetPos.y, speed), -1);
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
   
