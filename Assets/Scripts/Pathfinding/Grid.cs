using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private TextMesh[,] textArray;
    private Vector2 originPosition;
    private Tilemap wallTileMap;

    public Grid(int width, 
        int height, 
        float cellSize, 
        Vector2 originPosition, 
        Func<Grid<TGridObject>, int, int, TGridObject> createGridObject,
        Tilemap wallTileMap)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.wallTileMap = wallTileMap;

        gridArray = new TGridObject[width, height];
        textArray = new TextMesh[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 currentWorldPosition = GetWorldPosition(i, j);
                string formattedString = $"({i}, {j})";
                gridArray[i, j] = createGridObject(this, i, j);
                
                /*
                Helpers.CreateWorldText(
                                null,
                                formattedString,
                                currentWorldPosition,
                                100,
                                UnityEngine.Color.white,
                                TextAnchor.MiddleCenter,
                                TextAlignment.Center,
                                1);
                */
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public TGridObject GetGridObject(int x, int y)
    {
        return gridArray[x, y];
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        Vector2 worldPosition = new Vector2(x, y) * cellSize + originPosition;
        return worldPosition;
    }

    public void GetXY(Vector2 worldPosition, out int x, out int y)
    {
        x = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.RoundToInt((worldPosition - originPosition).y / cellSize);
    }

    public Vector2 GetXY(Vector2 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
        int y = Mathf.RoundToInt((worldPosition - originPosition).y / cellSize);
        return new Vector2(x, y);
    }

    public bool isTileWalkable(Vector2 currentWorldPosition)
    {
        Vector3Int cellPosition = wallTileMap.WorldToCell(currentWorldPosition);
        TileBase tile = wallTileMap.GetTile(cellPosition);

        if (tile != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}