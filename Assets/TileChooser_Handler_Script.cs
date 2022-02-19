using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileChooser_Handler_Script : MonoBehaviour
{
    [SerializeField] Image openCloseArrow;
    [SerializeField] float openPos;
    [SerializeField] float closePos;
    [SerializeField] float timeToMove = .2f;
    [SerializeField] RectTransform localTransform;
    // Start is called before the first frame update
    void Start()
    {
        close();
    }

    public void openOrClose()
    {
        //Debug.Log("move called:" + localTransform.position.x);
        if (localTransform.position.x > openPos - 1 && localTransform.position.x < openPos + 1)
        {
            //is open
            close();
        }
        else if (localTransform.position.x > closePos - 1 && localTransform.position.x < closePos + 1)
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

    [SerializeField]bool move = false;
    [SerializeField]bool openClose = false;

    public void Update()
    {
        if (move)
        {
            if (openClose)
            {
                //open
                if (localTransform.position.x >= openPos)
                {
                    Vector3 posreturn = localTransform.position;
                    posreturn.x = openPos;
                    //Debug.Log(posreturn);
                    localTransform.position = posreturn;
                    move = false;
                    openClose = false;
                    openCloseArrow.transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else
                {
                    Vector3 pos = localTransform.position;
                    pos.x += (openPos / timeToMove) * Time.deltaTime;
                    localTransform.position = pos;
                }
            }
            else
            {
                //close
                if (localTransform.position.x <= closePos)
                {
                    Vector3 posreturn = localTransform.position;
                    posreturn.x = closePos;
                    //Debug.Log(posreturn);
                    //Debug.Log("2:" + localTransform.position);
                    localTransform.position = posreturn;
                    //Debug.Log("3:" + localTransform.position);
                    move = false;
                    openClose = false;
                    openCloseArrow.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    Vector3 pos = localTransform.position;
                    pos.x += (closePos / timeToMove) * Time.deltaTime;
                    localTransform.position = pos;
                }
            }
        }
    }

}
