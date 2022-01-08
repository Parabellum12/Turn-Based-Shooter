using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilClass 
{


    //create world text
    public static TextMesh createWorldText(string text, Transform parent = null, Vector3 localPos = default(Vector3), int fontSize = 40, Color color = default(Color), TextAnchor textAnchor = TextAnchor.MiddleCenter, TextAlignment textAlignment = TextAlignment.Center, int sorintOrder = 0)
    {
        if (color == null)
        {
            color = Color.white;
        }
        return createWorldText(parent, text, localPos, fontSize, color, textAnchor, textAlignment, sorintOrder);
    }

    public static TextMesh createWorldText(Transform parent, string text, Vector3 localPos, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sorintOrder)
    {
        GameObject gameObject = new GameObject("WorldText", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPos;
        TextMesh textmesh = gameObject.GetComponent<TextMesh>();
        textmesh.anchor = textAnchor;
        textmesh.alignment = textAlignment;
        textmesh.text = text;
        textmesh.fontSize = fontSize;
        textmesh.color = color;
        textmesh.GetComponent<MeshRenderer>().sortingOrder = sorintOrder;
        return textmesh;
    }






    //get mouse world position
    public static Vector3 getMouseWorldPosition()
    {
        Vector3 vec = getMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 getMouseWorldPositionWithZ()
    {
        return getMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 getMouseWorldPositionWithZ(Camera worldCam)
    {
        return getMouseWorldPositionWithZ(Input.mousePosition, worldCam);
    }

    public static Vector3 getMouseWorldPositionWithZ(Vector3 screenPos, Camera worldCam)
    {
        Vector3 worldPos = worldCam.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}

