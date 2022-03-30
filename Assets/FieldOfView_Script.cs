using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView_Script : MonoBehaviour
{
    // Start is called before the first frame update
    public float fov = 90;
    public int rayCount = 2;
    public float viewDist = 10f;
    Mesh mesh;
    [SerializeField] LayerMask layerMask;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        updateFOVMesh();
    }

    public void updateFOVMesh()
    { 
        float currentAngle = 0f;
        float angleIncrease = fov / rayCount;




        Vector3[] verticies = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[verticies.Length];
        int[] triangles = new int[rayCount*3];
        Vector3 origin = Vector3.zero;
        verticies[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;

            RaycastHit2D raycaseHit = Physics2D.Raycast(origin, UtilClass.getVectorFromAngle(currentAngle), viewDist, layerMask);
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
    }
}
