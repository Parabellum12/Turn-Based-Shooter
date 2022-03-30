using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General2D_Raycast_Handler_Script : MonoBehaviour
{

    public IEnumerator getRayReturns(Vector2 origin, float zRot, Vector3 rot, float maxDist, int viewAngle, int NumOfPoints, System.Action<RaycastHit2D[]> callback = null)
    {
        float startTime = Time.realtimeSinceStartup;
        bool needToUpdate = false;

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.NameToLayer("WorldRaycasting");
        filter.useLayerMask = true;
        filter.useTriggers = false;


        float tempAngle = zRot - (viewAngle/2f);
        Vector3 currentAngle = RotationToVector(tempAngle, rot);
        float incrementAngleBy = (float)viewAngle / (float)NumOfPoints;


        for (int i = 0; i < NumOfPoints; i++)
        {
            if (needToUpdate)
            {
                startTime = Time.realtimeSinceStartup;
                needToUpdate = false;
            }
            hits.AddRange(castRay(origin, currentAngle, maxDist, filter));

            tempAngle += incrementAngleBy;
            currentAngle = RotationToVector(tempAngle, rot);
            Debug.Log("IncrementBy: "+ incrementAngleBy + " Angle: "+ tempAngle);
            if (Time.realtimeSinceStartup - startTime > 0.1)
            {
                needToUpdate = true;
                yield return null;
            }
        }

        callback.Invoke(hits.ToArray());
        yield break;
    }

    RaycastHit2D[] castRay(Vector2 origin, Vector3 angle, float maxDist, ContactFilter2D filter)
    {
        Debug.Log("RayCast: Origin:" + origin.ToString() + " Angle:" + angle.ToString());
        RaycastHit2D[] hits = new RaycastHit2D[1];
        Physics2D.Raycast(origin, angle, filter, hits, maxDist);


        Debug.DrawLine(origin, hits[0].point, Color.red, 10000);

        return hits;
    }

    Vector3 RotationToVector(float degrees, Vector3 rot)
    {

        Quaternion rotation = Quaternion.Euler(0, 0, degrees);
        Vector3 v = rotation * rot;

        return rotation.eulerAngles;
    }



}
