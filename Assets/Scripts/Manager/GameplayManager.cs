using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("_Debug")]
    [SerializeField] private bool drawGrid;
    [SerializeField] private bool drawOccupiedGrid;
    [SerializeField] private bool drawLowerLeftCorner;
    [SerializeField] private bool drawSpawnPosition;
    [SerializeField] private bool drawPreviewPosition;
#endif
    [Header("Gameplay Settings")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Vector2Int lowerLeftCorner;
    [SerializeField] Tetromino[] tetrominos;
    [SerializeField] Vector3Int spawnPosition;
    [SerializeField] Vector3 previewPosition;
    [SerializeField] float previewScale = 0.5f;
    [SerializeField] float difficultyScale = .9f;

    TetrisGrid grid;
    Tetromino currentTetromino;
    Tetromino nextTetromino;
    GameObject blocksOnGrid;
    float tetrominoSpeed = 1f;

    private void Start()
    {
        StartGame();
    }

    public bool IsMovementValid(Transform[] blocks)
    {
        Vector2Int position;

        for (int i = 0; i < blocks.Length; i++)
        {
            position = Vector2Int.RoundToInt(blocks[i].position);
            //Debug.LogFormat("Child index: {0} | Position: {1}", i, blocks[i].position);

            if (!grid.IsInside(position) || grid.IsOccupied(position))
                return false;
        }

        return true;
    }


    public void StartGame()
    {
        Debug.LogWarning("StartGame not fully implemented yet");
        Tetromino.OnMovementEnded += Tetromino_OnMovementEnded;

        blocksOnGrid = new GameObject("Blocks On Grid");

        CreateGrid();
        SpawnTetromino();
    }

    public void EndGame()
    {
        Debug.LogWarning("EndGame not fully implemented yet");
        Tetromino.OnMovementEnded -= Tetromino_OnMovementEnded;
    }

    public void CreateGrid()
    {
        grid = new TetrisGrid(width, height, lowerLeftCorner);
    }

    public void SpawnTetromino()
    {
        if (nextTetromino == null)
        {
            nextTetromino = GenerateTetromino();
        }

        currentTetromino = nextTetromino;
        currentTetromino.transform.position = spawnPosition;
        currentTetromino.transform.localScale = Vector3.one;

        nextTetromino = GenerateTetromino();
        nextTetromino.transform.position = previewPosition;
        nextTetromino.transform.localScale = Vector3.one * previewScale;

        currentTetromino.Initialize(tetrominoSpeed, this);
    }

    private Tetromino GenerateTetromino()
    {
        return Instantiate(tetrominos[UnityEngine.Random.Range(0, tetrominos.Length)], spawnPosition, Quaternion.identity);
    }

    private void Tetromino_OnMovementEnded(Tetromino tetromino)
    {
        StartCoroutine(OnMovementEndedRoutine(tetromino));
    }

    private IEnumerator OnMovementEndedRoutine(Tetromino tetromino)
    {
        foreach (var block in tetromino.Blocks)
        {
            if(!grid.IsInside(Vector2Int.RoundToInt(block.position), true))
            {
                EndGame();
                yield break;
            }
        }

        foreach (var block in tetromino.Blocks)
        {
            grid.Occupy(block);
            block.SetParent(blocksOnGrid.transform);
        }

        Destroy(tetromino.gameObject);

        bool willIncreaseDifficulty = false;
        while (grid.GetFullRow(out Transform[] blocks))
        {
            int y = (int)blocks[0].position.y;

            // Clear row that is full
            foreach (var block in blocks)
            {
                grid.Deoccupy(block.position);
                Destroy(block.gameObject);
            }



            // vvvvvv Estava fazendo isso vvvvvv
            // TODO: Detect all group of blocks above y
            Transform[] blocksAboveY = grid.GetAllBlocksAbove(y);


            // TODO: Move down all those groups
            foreach (var block in blocksAboveY)
            {
                grid.Deoccupy(block.position);
                block.transform.position += Vector3Int.down;
                grid.Occupy(block);
            }

            yield return new WaitForSeconds(difficultyScale);
            willIncreaseDifficulty = true;
        }

        if (willIncreaseDifficulty)
            IncreaseDifficulty();

        SpawnTetromino();
    }

    private void MoveDownBlocks(Transform[] blocks)
    {
        foreach (var block in blocks)
        {
            block.position += Vector3Int.down;
        }
    }

    private void IncreaseDifficulty()
    {
        tetrominoSpeed *= difficultyScale;
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            // Draw grid
            if (drawGrid)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        Gizmos.DrawWireCube(new Vector3(grid.StartPosition.x + x, grid.StartPosition.y + y), Vector3.one);
                    }
                }
            }

            // Draw occupied grid
            if (drawOccupiedGrid)
            {
                Gizmos.color = Color.red;
                for (int x = 0; x < grid.Width; x++)
                {
                    for (int y = 0; y < grid.Height; y++)
                    {
                        if (grid.IsOccupied(new Vector2Int(x, y)))
                        {
                            Gizmos.DrawWireCube(new Vector3(x, y), Vector3.one);
                        }
                    }
                }
            }
        }

        Gizmos.color = Color.cyan;

        // Draw lowerLeftCorner
        if (drawLowerLeftCorner)
        {
            Gizmos.DrawWireCube(new Vector3(lowerLeftCorner.x, lowerLeftCorner.y), Vector3.one);
        }

        // Draw Spawn Position
        if (drawSpawnPosition)
        {
            Gizmos.DrawWireCube(spawnPosition, new Vector3(5, 5));
        }

        // Draw Preview Position
        if (drawPreviewPosition)
        {
            Gizmos.DrawWireCube(previewPosition, new Vector3(5, 5) * previewScale);
        }
    }
}
