using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Icon_Script : MonoBehaviour
{
    [SerializeField] TMP_Text userName;
    [SerializeField] Image colorBar;


    public void setUserName(string name)
    {
        userName.text = name;
    }

    public void Ready()
    {
        colorBar.color = Color.green;
    }

    public void NotReady()
    {
        colorBar.color = Color.red;
    }

    public void deleteMe()
    {
        Destroy(gameObject);
    }

    public void updateColorBarSize()
    {
        colorBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
        colorBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, userName.gameObject.GetComponentInParent<RectTransform>().rect.height);
    }
}
