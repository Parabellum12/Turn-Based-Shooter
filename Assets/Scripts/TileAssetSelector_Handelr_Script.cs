using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileAssetSelector_Handelr_Script : MonoBehaviour
{
    public Image AssetImage;
    public Button AssetSelectButton;
    public TileBuildData BuildData;
    public TileChooser_Handler_Script handler;


    public void SetSelectorData()
    {
        AssetImage.sprite = BuildData.buildingSprite;
        AssetSelectButton.onClick.AddListener(callHandlerChange);
    }
    public void deleteMe()
    {
        Destroy(gameObject);
    }

    public Sprite getSprite()
    {
        return BuildData.buildingSprite;
    }

    public void callHandlerChange()
    {
        handler.CurrentSelectedTile = BuildData;
    }
}
