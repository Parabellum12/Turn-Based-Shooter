using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileChooser_Handler_Script : MonoBehaviour
{
    [SerializeField] Image openCloseArrow;
    [SerializeField] TMP_Text assetClassLabel;
    float openPos;
    float closePos;
    float timeToMove = .1f;
    [SerializeField] RectTransform localTransform;
    private float moveDist;
    // Start is called before the first frame update
    void Start()
    {
        closePos = localTransform.localPosition.x;
        openPos = closePos + localTransform.rect.width;
        moveDist = Mathf.Abs(openPos - closePos);
        //Debug.Log(closePos + ";" + openPos);
        close();
    }

    public void openOrClose()
    {
        //Debug.Log("move called:" + localTransform.localPosition.x);
        if (localTransform.localPosition.x > openPos - 1 && localTransform.localPosition.x < openPos + 1)
        {
            //is open
            close();
        }
        else if (localTransform.localPosition.x > closePos - 1 && localTransform.localPosition.x < closePos + 1)
        {
            //is closed
            open();
        }
    }

    public void open()
    {
        //Debug.Log("open called");
        move = true;
        openClose = true;
    }

    public void close()
    {
        //Debug.Log("close called");
        move = true;
        openClose = false;
    }

    bool move = false;
    bool openClose = false;

    public void Update()
    {
        if (move)
        {
            if (openClose)
            {
                //open
                if (localTransform.localPosition.x >= openPos)
                {
                    Vector2 posreturn = localTransform.localPosition;
                    posreturn.x = openPos;
                    //Debug.Log("open ended");
                    localTransform.localPosition = posreturn;
                    move = false;
                    openClose = false;
                    openCloseArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else
                {
                    Vector2 pos = localTransform.localPosition;
                    pos.x += (moveDist / timeToMove) * Time.deltaTime;
                    localTransform.localPosition = pos;
                }
            }
            else
            {
                //close
                if (localTransform.localPosition.x <= closePos)
                {
                    Vector2 posreturn = localTransform.localPosition;
                    posreturn.x = closePos;
                    //Debug.Log("close ended");
                    localTransform.localPosition = posreturn;
                    move = false;
                    openClose = false;
                    openCloseArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    Vector2 pos = localTransform.localPosition;
                    pos.x -= (moveDist / timeToMove) * Time.deltaTime;
                    localTransform.localPosition = pos;
                }
            }
        }
    }



    //tile Asset Selection Handling

    [SerializeField] TileBuildData[] GroundTiles;
    List<TileAssetSelector_Handelr_Script> CurrentlyVisibleTileAssets;
    public void setSelectedTileType()
    {

    }



}
