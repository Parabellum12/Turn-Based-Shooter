using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockTransformToParent : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        transform.rotation.SetEulerAngles(0, 0, -1*transform.parent.rotation.z);
    }
}
