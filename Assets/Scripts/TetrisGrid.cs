using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid
{
    private readonly struct Bounds
    {
        public int Left { get; }
        public int Right { get; }
        public int Up { get; }
        public int Down { get; }

        public Bounds(int left, int right, int up, int down)
        {
            Left = left;
            Right = right;
            Up = up;
            Down = down;
        }
    }


    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2Int StartPosition { get; private set; }


    readonly Transform[,] grid;
    Bounds bounds;

    public TetrisGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new Transform[width, height];
        StartPosition = Vector2Int.zero;
        SetBounds();
    }

    public TetrisGrid(int width, int height, Vector2Int startPosition) : this(width, height)
    {
        StartPosition = startPosition;
        SetBounds();
    }

    public Transform GetTransform(int x, int y)
    {
        return grid[x, y];
    }

    public bool IsInside(Vector2Int worldPosition, bool includeUpBound = false)
    {
        if (worldPosition.x < bounds.Left || worldPosition.x > bounds.Right || worldPosition.y < bounds.Down)
            return false;
        if (includeUpBound && worldPosition.y > bounds.Up)
            return false;
        return true;
    }

    public bool IsOccupied(Vector2Int worldPosition)
    {
        Vector2Int gridIndex = worldPosition - StartPosition;
        if (gridIndex.x < 0 || gridIndex.x > grid.GetLength(0) - 1 || gridIndex.y < 0 || gridIndex.y > grid.GetLength(1) - 1)
        {
            // it is outside the grid boundaries, so it must be a free space
            return false;
        }
        return grid[gridIndex.x, gridIndex.y] != null;
    }

    public void Occupy(Transform obj)
    {
        Vector2Int position = Vector2Int.RoundToInt(obj.position);
        try
        {
            grid[position.x, position.y] = obj;
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat(
                "From Occupy: Position {0} IS NOT INSIDE GRID! Valid values: {1}< x <{2} AND {3}< y <{4}",
                position, bounds.Left, bounds.Right, bounds.Down, grid.GetLength(1)-1);
        }
    }

    public Transform Deoccupy(Vector2 position)
    {
        Vector2Int index = Vector2Int.RoundToInt(position);
        Transform obj = grid[index.x, index.y];
        grid[index.x, index.y] = null;

        return obj;
    }

    public Transform[] GetBlocks(int y)
    {
        List<Transform> blocks = new List<Transform>();
        for (int x = 0; x < Width; x++)
        {
            Transform block = grid[x, y];
            if (block != null)
            {
                blocks.Add(block);
            }
        }

        return blocks.ToArray();
    }

    public Transform[] GetAllBlocksAbove(int y)
    {
        List<Transform> blocks = new List<Transform>();
        y++;

        for (int i = y; i < Height; i++)
        {
            foreach (var block in GetBlocks(i))
            {
                blocks.Add(block);
            }
        }
        // TODO: Fazer o array dos blocos encontrados

        return blocks.ToArray();
    }

    public bool GetFullRow(out Transform[] row)
    {
        bool hasFullRow = false;
        row = null;

        for (int y = 0; y < Height; y++)
        {
            int count = 0;

            for (int x = 0; x < Width; x++)
            {
                if (!IsOccupied(new Vector2Int(x, y)))
                    break;
                else
                    count++;
            }

            if (count >= Width)
            {
                // has full row
                hasFullRow = true;
                row = GetBlocks(y);

                break;
            }
        }

        return hasFullRow;
    }

    public void ClearRow(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            Transform obj = Deoccupy(new Vector2Int(x, y));
            Object.Destroy(obj);
        }
    }

    private void SetBounds()
    {
        bounds = new Bounds(StartPosition.x, StartPosition.x + Width - 1, StartPosition.y + Height - 1, StartPosition.y);
        //Debug.LogFormat("Bounds: Left({0}), Right({1}), Down({2})", bounds.Left, bounds.Right, bounds.Down);
    }
}
