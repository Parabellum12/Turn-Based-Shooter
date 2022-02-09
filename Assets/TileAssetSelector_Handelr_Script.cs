using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileAssetSelector_Handelr_Script : MonoBehaviour
{
    public Image AssetImage;
    public Button AssetSelectButton;
    public RectTransform AssetSelectRectTransform;


    public void DeleteMe()
    {
        Destroy(gameObject);
    }
}
