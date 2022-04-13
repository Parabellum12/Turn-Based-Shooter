using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet_Handler_Script : MonoBehaviourPun
{
    Vector2 targetPos;
    bool moving = false;
    [SerializeField] float speed = 15000;
    float xMoveBy;
    float yMoveBy;
    [SerializeField] PhotonView localview;
    bool closeOrFar;
    public void setTarget(Vector2 origin, Vector2 target, bool closeOrFar)
    {
        if (!localview.IsMine)
        {
            return;
        }
        //true = close, false = far
        this.closeOrFar = closeOrFar;
        targetPos = target;

        float targetAngle = UtilClass.getAngleFromVectorFloat(target - origin);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, targetAngle+90);

        Vector3 angleVec = UtilClass.getVectorFromAngle(targetAngle);
        xMoveBy = angleVec.x;
        yMoveBy = angleVec.y;
        moving = true;
    }

    private void Update()
    {
        if (localview.IsMine && moving)
        {
            Vector2 test = new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Distance(test, targetPos) < 1f)
            {
                moving = false;
            }
            else
            {
                transform.position += new Vector3(xMoveBy * speed * Time.deltaTime, yMoveBy * speed * Time.deltaTime, 0);
            }
        }
        else if (localview.IsMine && !moving)
        {
            PhotonNetwork.Destroy(gameObject);
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!localview.IsMine)
        {
            return;
        }
        //Debug.Log("trigger Entered");
        if (collision.gameObject.tag == ("EnemyUnit"))
        {
            moving = false;
            if (closeOrFar)
            {
                collision.gameObject.GetComponent<InGame_Unit_Handler_Script>().handleGettingShot();
            }
            else
            {
                collision.gameObject.GetComponent<InGame_Unit_Handler_Script>().handleGettingShot();
            }
        }
    }

}
