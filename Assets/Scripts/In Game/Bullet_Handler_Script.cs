using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Handler_Script : MonoBehaviour
{
    Vector2 targetPos;
    bool moving = false;
    [SerializeField] float speed = 15000;
    float xMoveBy;
    float yMoveBy;

    public void setTarget(Vector2 origin, Vector2 target)
    {
        targetPos = target;

        float targetAngle = UtilClass.getAngleFromVectorFloat(target - origin);
        transform.rotation.SetEulerAngles(transform.rotation.x, transform.rotation.y, targetAngle);

        Vector3 angleVec = UtilClass.getVectorFromAngle(targetAngle);
        xMoveBy = angleVec.x;
        yMoveBy = angleVec.y;
        moving = true;
    }

    private void Update()
    {
        if (moving)
        {
            Vector2 test = new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Distance(test, targetPos) < 0.05f)
            {
                Destroy(this.gameObject);
                moving = false;
            }
            else
            {
                transform.position += new Vector3(xMoveBy * speed * Time.deltaTime, yMoveBy * speed * Time.deltaTime, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger Entered");
        if (collision.gameObject.tag.Equals("EnemyUnit"))
        {
            Destroy(this.gameObject);
        }
    }
}
