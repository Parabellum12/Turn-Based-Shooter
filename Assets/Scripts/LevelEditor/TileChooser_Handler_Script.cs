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
        setSelectedTileType();
        CurrentSelectedTile = CurrentlyVisibleTileAssets[0].BuildData;
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
    public void AssetClassTypeSwitchLeft()
    {

    }

    public void AssetClassTypeSwitchRight()
    {

    }

    [SerializeField] TileBuildData[] GroundTiles;
    List<TileAssetSelector_Handelr_Script> CurrentlyVisibleTileAssets = new List<TileAssetSelector_Handelr_Script>();
    [SerializeField] RectTransform ClassTypeTextBanner;
    [SerializeField] GameObject TileAssetSelecterPrefab;
    public TileBuildData CurrentSelectedTile;
    public TileBuildData defaultTile;
    Dictionary<string, int> tileNameToIndex = new Dictionary<string, int>()
    {
        {"Ground", 0}
    };

    private TileBuildData[] getTileTypeArray()
    {
        switch (assetClassLabel.text)
        {
            case "Ground":
                return GroundTiles;
        }
        return null;
    }

    private Vector2 getTopLeftCorner()
    {
        Vector2 returner = Vector2.zero;
        returner.x -= localTransform.rect.width / 2f;
        returner.y += (localTransform.rect.height / 2f) - ClassTypeTextBanner.rect.height;
        return returner;
    }

    private Vector2 getLocalPosToSet(int index, Vector2 TLC)
    {
        index++;
        Debug.Log("index: "+index);
        int x = (index % 2);
        if (x == 1)
        {
            TLC.x += (localTransform.rect.width / 3);
        }
        else
        {
            TLC.x += (localTransform.rect.width / 3) * 2;
        }
        int y = Mathf.CeilToInt(index / 2f);
        Debug.Log("y:"+y);
        TLC.y -= ((TileAssetSelecterPrefab.GetComponent<RectTransform>().rect.height) * 1.1f) * y;
        return TLC;
    }
    public void setSelectedTileType()
    {
        foreach (TileAssetSelector_Handelr_Script scr in CurrentlyVisibleTileAssets)
        {
            scr.deleteMe();
        }
        CurrentlyVisibleTileAssets = new List<TileAssetSelector_Handelr_Script>();
        tileNameToIndex.TryGetValue(assetClassLabel.text, out int index);
        Vector2 TLC = getTopLeftCorner();
        TileBuildData[] tileTypeArray = getTileTypeArray();
        for (int i = 0; i < tileTypeArray.Length; i++)
        {
            GameObject temp = Instantiate(TileAssetSelecterPrefab, TLC, Quaternion.identity, gameObject.transform);
            temp.GetComponent<RectTransform>().localPosition = getLocalPosToSet(i, TLC);
            TileAssetSelector_Handelr_Script tempScript = temp.GetComponent<TileAssetSelector_Handelr_Script>();
            tempScript.BuildData = tileTypeArray[i];
            tempScript.SetSelectorData();
            tempScript.handler = this;
            CurrentlyVisibleTileAssets.Add(tempScript);
        }

    }



}
