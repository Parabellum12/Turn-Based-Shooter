

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTileVisual : MonoBehaviour {

    private GridClass<World_Handler_Script.WorldBuildTile> objectGrid;
    private Mesh mesh;
    private bool updateMesh;


    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(GridClass<World_Handler_Script.WorldBuildTile> objectGrid) {
        this.objectGrid = objectGrid;
        UpdateHeatMapVisual();
        objectGrid.onGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, GridClass<World_Handler_Script.WorldBuildTile>.OnGridValueChangedEventArgs e) {
        updateMesh = true;
    }

    private void LateUpdate() {
        if (updateMesh) {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    private void UpdateHeatMapVisual() {
        MeshUtils.CreateEmptyMeshArrays(objectGrid.getWidth() * objectGrid.getWidth(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
        for (int x = 0; x < objectGrid.getWidth(); x++) {
            for (int y = 0; y < objectGrid.getHeight(); y++) {
                int index = x * objectGrid.getHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * objectGrid.getCellSize();

                World_Handler_Script.WorldBuildTile gridObject = objectGrid.getGridObject(x, y);
                Sprite TileSprite = gridObject.getMainBuildData().buildingSprite;

                //Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                float width = TileSprite.texture.width;
                float height = TileSprite.texture.height;
                Vector2[] uvCoords = TileSprite.uv;
                Vector2 uv00 = uvCoords[3];
                Vector2 uv11 = uvCoords[2];
                for (int i = 0; i < uvCoords.Length; i++)
                {
                    Debug.Log("UvCoords of"+ gridObject.getMainBuildData().BuildingName +":"+uvCoords[i]);
                }
                Debug.Log("End UvCoords of" + gridObject.getMainBuildData().BuildingName);

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, objectGrid.getWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, uv00, uv11);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

}


