using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView_Script : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float fov = 120;
    [SerializeField] public int rayCount = 2;
    [SerializeField] public float viewDist = 10f;
    Mesh mesh;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask layerMask2;
    [SerializeField] public Transform lockOnTo = null;

    Vector3 origin;
    void Start()
    {
        mesh = new Mesh();
        origin = transform.position;
        transform.position = Vector3.zero;
        GetComponent<MeshFilter>().mesh = mesh; 
        //layerMask = LayerMask.NameToLayer("WorldRaycasting");
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 90 + (fov/2));
        updateFOVMesh();
        

    }

    

    private void LateUpdate()
    {
        if (lockOnTo != null)
        {
            setAimDirection();
            origin = lockOnTo.position;
            //transform.position = origin;
            //transform.position = lockOnTo.position;
        }
        updateFOVMesh();
    }
    float startingAngle = 0f;



    public List<GameObject> currentlySeenUnits = new List<GameObject>();

    public void updateFOVMesh()
    { 
        float currentAngle = startingAngle;
        float angleIncrease = fov / rayCount;




        Vector3[] verticies = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[verticies.Length];
        int[] triangles = new int[rayCount*3];


        verticies[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        currentlySeenUnits.Clear();
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;

            RaycastHit2D raycaseHit = Physics2D.Raycast(origin, UtilClass.getVectorFromAngle(currentAngle), viewDist, layerMask);
            RaycastHit2D raycaseHit2 = Physics2D.Raycast(origin, UtilClass.getVectorFromAngle(currentAngle), viewDist, layerMask2);


            if (raycaseHit2.collider != null && raycaseHit2.collider.gameObject.CompareTag("FriendlyUnit"))
            {
                //Debug.Log("I See Enemy!");
                GameObject curScr = raycaseHit2.collider.gameObject;
                if (!currentlySeenUnits.Contains(curScr))
                {
                    currentlySeenUnits.Add(curScr);
                }
            }


            //Debug.DrawRay(origin, UtilClass.getVectorFromAngle(currentAngle) * viewDist, Color.green, .1f);
            if (raycaseHit.collider != null)
            {
                //hit
                vertex = raycaseHit.point;
                
            }
            else
            {
                //no hit
                vertex = origin + UtilClass.getVectorFromAngle(currentAngle) * viewDist;
            }



            verticies[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            currentAngle -= angleIncrease;
        }





        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

   

    private void setAimDirection()
    {
        startingAngle = lockOnTo.rotation.eulerAngles.z + 90 + (fov/2f);
    }

    public void setViewDist(float dist)
    {
        viewDist = dist;
    }

    public void setFOV(float fov)
    {
        this.fov = fov;
        rayCount = Mathf.RoundToInt(fov) * 2;
    }

    public void setParameters(float fov, int rayCount, float viewDist)
    {
        setFOV(fov);
        setViewDist(viewDist);
    }

    

}
