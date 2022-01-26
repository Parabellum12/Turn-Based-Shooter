using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanAndZoom_Script : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float mouseSensitivityPercent = 100;
    [SerializeField] float maxCameraDist = 25;
    [SerializeField] float minCameraDist = 1;
    [SerializeField] float scrollSensitivityPercent = 100;

    bool isRightMouseDown = false;
    Vector2 MouseStartingPos;

    private void Start()
    {
        mainCamera.transform.position = new Vector3(0,0,-10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MouseStartingPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isRightMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
        }






        HandleCameraPan();
    }

    void HandleCameraPan()
    {
        if (isRightMouseDown)
        {
            Vector2 newMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            float changeInX = newMousePos.x - MouseStartingPos.x;
            float changeInY = newMousePos.y - MouseStartingPos.y;

            Vector3 newCameraPos = Vector2.zero;
            newCameraPos.x = mainCamera.transform.position.x - (changeInX * (mouseSensitivityPercent / 100));
            newCameraPos.y = mainCamera.transform.position.y - (changeInY * (mouseSensitivityPercent / 100));
            newCameraPos.z = -10;
            mainCamera.transform.position = newCameraPos;

            MouseStartingPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        //camera zoom
        float size = mainCamera.orthographicSize;
        size -= Input.mouseScrollDelta.y * (scrollSensitivityPercent / 100);
        //Debug.Log(Input.mouseScrollDelta.y);
        mainCamera.orthographicSize = Mathf.Clamp(size, minCameraDist, maxCameraDist);

    }
}
