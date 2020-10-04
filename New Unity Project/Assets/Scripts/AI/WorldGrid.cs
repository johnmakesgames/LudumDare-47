using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    TILETYPE_EMPTY,
    TILETYPE_EMPTY_CHECKED,
    TILETYPE_BLOCKED,
    TILETYPE_PATH
};

public class WorldGrid : MonoBehaviour
{
    public int GridSquareSize;
    public float GridWidth;
    public float GridHeight;
    public GameObject GridCollisionObject;
    public bool RenderGrid = true;
    public List<GameObject> GridColliders;
    public TileType[,] gridMap;
    public bool finishedLoading = false;
    private float timeRunning = 0;

    public void SetGridSizes()
    {
        gridMap = new TileType[Convert.ToInt32(GridHeight), Convert.ToInt32(GridWidth)];
    }

    public void GenerateCollisionGrid()
    {
        float widthToCreate = GridWidth / GridSquareSize;
        float heightToCreate = GridHeight / GridSquareSize;
        for (float x = 0; x < widthToCreate; x++)
        {
            for (float z = 0; z < heightToCreate; z++)
            {
                var collider = GameObject.Instantiate(GridCollisionObject);
                collider.transform.position = this.transform.position + new Vector3(x * GridSquareSize, 0, z * GridSquareSize);
                collider.transform.localScale = new Vector3(GridSquareSize, GridSquareSize, GridSquareSize);
                collider.GetComponent<Renderer>().enabled = true;
                collider.GetComponent<GridCube>().GridPosition = new Vector2(x, z);
                GridColliders.Add(collider);
            }
        }
    }

    public void CreateNavGrid()
    {
        foreach(var collider in GridColliders)
        {
            var gridItem = collider.GetComponent<GridCube>();
            TileType type = (gridItem.IsColliding) ? TileType.TILETYPE_BLOCKED : TileType.TILETYPE_EMPTY;

            gridMap[Convert.ToInt32(gridItem.GridPosition.x), Convert.ToInt32(gridItem.GridPosition.y)] = type;
        }
    }

    public void UpdateNavGrid()
    {
        foreach (var collider in GridColliders)
        {
            var gridItem = collider.GetComponent<GridCube>();

            TileType type = (gridItem.IsColliding) ? TileType.TILETYPE_BLOCKED : TileType.TILETYPE_EMPTY;

            gridMap[Convert.ToInt32(gridItem.GridPosition.x), Convert.ToInt32(gridItem.GridPosition.y)] = type;
        }

        finishedLoading = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetGridSizes();
        GenerateCollisionGrid();
        CreateNavGrid();
    }

    void Update()
    {
        if (timeRunning < 25)
        {
            UpdateNavGrid();
            timeRunning += Time.deltaTime;
        }
        else
        {
            foreach (var item in GridColliders)
            {
                item.SetActive(false);
            }
        }
    }
}
