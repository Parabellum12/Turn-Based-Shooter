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
    [SerializeField] GameObject showSelfMask;





    private void Start()
    {
        gameHadlerObj = GameObject.FindGameObjectWithTag("GameController");
        gameHandlerScript = gameHadlerObj.GetComponent<Game_Handler>();
        if (localview.IsMine)
        {
            showSelfMask.SetActive(true);
            setuplocalViewSys();
            StartCoroutine(HandleStopMovement());
            currentHealth = characterData.HealthPoints;
            gameObject.layer = LayerMask.NameToLayer("behindMask2");
        }
    }

    public bool isSelected = false;
    bool HeadingToWhiteOrGreen = true;
    float time = 0;
    private void Update()
    {
        if (needToMove)
        {
            transform.position = new Vector3(HandleMoveSingleAxis(transform.position.x, targetPos.x, speed), HandleMoveSingleAxis(transform.position.y, targetPos.y, speed), -1);
        }
        if (localview.IsMine && isSelected)
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                HeadingToWhiteOrGreen = !HeadingToWhiteOrGreen;
                time -= 1;
            }
            if (HeadingToWhiteOrGreen)
            {
                //white
               // Debug.Log("Flash");
                indicator.color = Color.Lerp(Color.green, Color.white, time);
            }
            else
            {
                //green
                //Debug.Log("why Flash");
                indicator.color = Color.Lerp(Color.white, Color.green, time);
            }
        }
        else if (!localview.IsMine)
        {
            indicator.color = Color.red;
            //Debug.Log("Stop Flash");
        }
        else if (!isSelected)
        {
            indicator.color = Color.green;
        }

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
    bool needToHideOtherSelf = false;
     
    [PunRPC] void hideSelfIfNotOwner()
    {
        needToHideOtherSelf = true;
    }

    public void hideOtherSelf()
    {
        localview.RPC("hideSelfIfNotOwner", RpcTarget.Others);
    }


    public void setup(Vector3 pos, CharacterData charDat, Vector2Int gridpos)
    {
        setuplocalViewSys();
        gameObject.tag = "FriendlyUnit";
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
                localViewSystem.setRayCount(280);
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
                localViewSystem.setRayCount(360);
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
        if (currentActionPoints - (gameHandlerScript.worldHandler.getBuildLayers().getGridObject(posList[0].x, posList[0].y).gCost * getMoveCostMultiplier()) < 0)
        {
            yield break;
        }

        //Debug.Log("Move");
        Vector2Int finalPos = gridPos;
        foreach (Vector2Int vec in posList)
        {
            if (currentActionPoints - (gameHandlerScript.worldHandler.getBuildLayers().getGridObject(vec.x, vec.y).gCost * getMoveCostMultiplier()) < 0)
            {
                break;
            }
            finalPos = vec;
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

        currentActionPoints -= gameHandlerScript.worldHandler.getBuildLayers().getGridObject(finalPos.x, finalPos.y).gCost / 2;
        needToMove = false;
        gameHandlerScript.worldHandler.getBuildLayers().GetXY(transform.position, out int x, out int y);
        seGridPos(new Vector2Int(x, y));
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



    

    bool seenUnitsAlready = false;
    [SerializeField] GameObject[] seenStuff = null;
    IEnumerator HandleStopMovement()
    {
        while (true)
        {
            seenStuff = localViewSystem.currentlySeenUnits.ToArray();
            if (needToHideOtherSelf)
            {
                yield break;
            }
            if (localViewSystem.currentlySeenUnits.Count > 0)
            {
                if (!seenUnitsAlready)
                {
                    seenUnitsAlready = true;

                    if (needToMove)
                    {
                        yield return StartCoroutine(stopMovement());
                    }
                }
            }
            else
            {
                seenUnitsAlready = false;
            }


            yield return null;
        }
    }



    private void OnMouseDown()
    {
        if (!localview.IsMine)
        {
            gameHandlerScript.selectedEnemyUnit = this;
        }
    }






    //turn stuff
    public int currentHealth;
    public int currentActionPoints;

    public void resetValuesOnStartOfTurn()
    {
        currentActionPoints = characterData.ActionPoints;
    }

    public int getRemainingAPAfterPath(float gcost)
    {
        return 0;
    }



    public List<InGame_Unit_Handler_Script> getSeenEnemyUnits()
    {
        List<InGame_Unit_Handler_Script> returner = new List<InGame_Unit_Handler_Script>();
        foreach (GameObject gm in localViewSystem.currentlySeenUnits)
        {
            returner.Add(gm.GetComponent<InGame_Unit_Handler_Script>());
        }
        return returner;
    }
    public List<InGame_Unit_Handler_Script> getSeenEnemyUnitsFar()
    {
        List<InGame_Unit_Handler_Script> returner = new List<InGame_Unit_Handler_Script>();
        foreach (GameObject gm in localViewSystem.currentlySeenUnitsFar)
        {
            returner.Add(gm.GetComponent<InGame_Unit_Handler_Script>());
        }
        return returner;
    }

    public float getMoveCostMultiplier()
    {
        return .5f;
    }


    public void handleGettingShot()
    {
        localview.RPC("ownerHandleGetShot", localview.Owner);
    }

    [PunRPC] void ownerHandleGetShot()
    {
        currentHealth -= Mathf.FloorToInt(10f * (1f - ((characterData.Armor/2f)/100f)));
    }

    public void handleDeath()
    {
        Destroy(localViewSystem.gameObject);
        PhotonNetwork.Destroy(this.gameObject);
    }

}
   
