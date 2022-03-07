using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCameraHandler_Script : MonoBehaviour
{
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float acceleration = 1f;
    float horizonalSpeed = 0;
    float verticalSpeed = 0;
    public void Update()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");
        horizonalSpeed += xMove * acceleration * Time.deltaTime;
        verticalSpeed += yMove * acceleration * Time.deltaTime;
        if (yMove == 0)
        {
            verticalSpeed = 0;
        }
        if (xMove == 0)
        {
            horizonalSpeed = 0;
        }












        horizonalSpeed = Mathf.Clamp(horizonalSpeed, -maxSpeed, maxSpeed);
        verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeed, maxSpeed);
        float newx = transform.position.x + horizonalSpeed;
        float newy = transform.position.y + verticalSpeed;
        Debug.Log(xMove + "," + yMove + "::" + horizonalSpeed + "," + verticalSpeed);
        transform.position = new Vector2(newx, newy);
    }
}
