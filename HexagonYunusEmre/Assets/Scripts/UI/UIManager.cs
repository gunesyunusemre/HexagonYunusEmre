using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    #region Variables
    
    #region Text Components
    [SerializeField] private Text HighScoreText;
    [SerializeField] private Text ScoreText;
    [SerializeField] private Text MoveText;
    #endregion

    #region Gameover Componenets
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private Button TryAgainButton;
    #endregion
    
    #endregion

    #region MonoBehaviour

    void Start()
    {
        WriteHighScoreText();
        WriteMoveText();
        WriteScoreText();

        GameCell.OnMoveCountUpEventHandler += WriteMoveText;
        GameCell.OnScoreUpEventHandler += WriteScoreText;
        Cell.GameOverEventHandler += GameOver;
        
        TryAgainButton.onClick.AddListener(TryAgain);
    }

    private void OnDisable()
    {
        GameCell.OnMoveCountUpEventHandler -= WriteMoveText;
        GameCell.OnScoreUpEventHandler -= WriteScoreText;
        Cell.GameOverEventHandler += GameOver;
    }

    #endregion

    #region WriteFuncs.

    private void WriteScoreText()
    {
        ScoreText.text = Player._player.Score.ToString();
    }
    
    private void WriteHighScoreText()
    {
        HighScoreText.text = Player._player.HighScore.ToString();
    }
    
    private void WriteMoveText()
    {
        MoveText.text = Player._player.MoveCounter.ToString();
    }

    #endregion

    #region GameOverScreen

    private void GameOver()
    {
        GameOverScreen.SetActive(true);
    }

    private void TryAgain()
    {
        SceneManager.LoadScene("LevelScene");
    }

    #endregion
    
    
}
