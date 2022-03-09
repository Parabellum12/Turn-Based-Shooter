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
        int runMulti = 1;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            runMulti = 2;
        }
        horizonalSpeed = xMove * acceleration * runMulti * Time.deltaTime;
        verticalSpeed = yMove * acceleration * runMulti * Time.deltaTime;
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
        //Debug.Log(xMove + "," + yMove + "::" + horizonalSpeed + "," + verticalSpeed);
        transform.position = new Vector3(newx, newy, -10);
    }
}
