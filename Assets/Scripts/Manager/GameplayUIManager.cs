using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private TextUI gameplayScoreUI;
    [SerializeField] private TextUI gameplayLevelUI;
    [SerializeField] private Canvas gameplayCanvas;

    [Header("Game Over")]
    [SerializeField] private TextUI gameOverScoreUI;
    [SerializeField] private Canvas gameOverCanvas;

    public int Score { set { gameplayScoreUI.SetText(value.ToString()); } }
    public int Level { set { gameplayLevelUI.SetText(value.ToString()); } }

    public void ChangeToGameplayUI()
    {
        gameplayCanvas.enabled = true;
        gameOverCanvas.enabled = false;
    }

    public void ChangeToGameOverUI(int finalScore)
    {
        gameplayCanvas.enabled = false;
        gameOverCanvas.enabled = true;
        gameOverScoreUI.SetText(finalScore.ToString());
    }
}
