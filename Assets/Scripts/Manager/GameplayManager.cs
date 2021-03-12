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
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Vector2Int lowerLeftCorner;
    [SerializeField] private Tetromino[] tetrominos;
    [SerializeField] private Vector3Int spawnPosition;
    [SerializeField] private Vector3 previewPosition;
    [SerializeField] private float previewScale = 0.5f;
    [SerializeField] private float difficultyScale = .9f;

    [Header("Score and Level")]
    [SerializeField] private TextUI scoreUI;
    [SerializeField] private int scoreBasePoints = 50;
    [SerializeField] private TextUI levelUI;

    [Header("Extras")]
    [SerializeField] private EffectsManager effects;

    private TetrisGrid grid;
    private Tetromino currentTetromino;
    private Tetromino nextTetromino;
    private GameObject blocksOnGrid;
    private float tetrominoSpeed = 1f;
    private int level;
    private int score;

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
        SubscribeEvents();

        blocksOnGrid = new GameObject("Blocks On Grid");

        score = 0;
        scoreUI.SetText(score.ToString());
        level = 1;
        levelUI.SetText(level.ToString());

        CreateGrid();
        SpawnTetromino();
    }

    public void EndGame()
    {
        Debug.LogWarning("EndGame not fully implemented yet");
        UnsubscribeEvents();

        effects?.PlayAudio(AudioType.GameOver);
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
            // Check end game (game over)
            if (!grid.IsInside(Vector2Int.RoundToInt(block.position), true))
            {
                EndGame();
                yield break;
            }
            else
            {
                // Occupy grid
                grid.Occupy(block);
                block.SetParent(blocksOnGrid.transform);
            }
        }

        tetromino.Destroy();

        bool willIncreaseDifficulty = false;
        int rowsCleared = 0;
        while (grid.GetFullRow(out Transform[] blocks))
        {
            int y = (int)blocks[0].position.y;

            // Clear row that is full
            foreach (var block in blocks)
            {
                ClearBlock(block);
            }
            effects?.PlayAudio(AudioType.RowClearing);

            Transform[] blocksAboveY = grid.GetAllBlocksAbove(y);
            foreach (var block in blocksAboveY)
            {
                MoveDownBlock(block);
            }

            yield return new WaitForSeconds(tetrominoSpeed);
            willIncreaseDifficulty = true;
            rowsCleared++;
        }

        if (rowsCleared > 0)
            UpdateScore(rowsCleared);

        if (willIncreaseDifficulty)
            IncreaseDifficulty();

        SpawnTetromino();
    }

    private void ClearBlock(Transform block)
    {
        grid.Deoccupy(block.position);
        Destroy(block.gameObject);
    }

    private void MoveDownBlock(Transform block)
    {
        grid.Deoccupy(block.position);
        block.transform.position += Vector3Int.down;
        grid.Occupy(block);
    }

    private void UpdateScore(int rowsCleared)
    {
        score += scoreBasePoints * rowsCleared * level;
        scoreUI.SetText(score.ToString());
    }

    private void IncreaseDifficulty()
    {
        tetrominoSpeed *= difficultyScale;

        level++;
        levelUI.SetText(level.ToString());
    }

    private void SubscribeEvents()
    {
        Tetromino.OnEnd += Tetromino_OnMovementEnded;
    }

    private void UnsubscribeEvents()
    {
        Tetromino.OnEnd -= Tetromino_OnMovementEnded;
    }

    

    private void Start()
    {
        StartGame();
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
