using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField]
    protected Text playerText;
    [SerializeField]
    protected Text scoreText;
    [SerializeField]
    protected Text bestScoreText;
    [SerializeField]
    protected Text levelText;
    [SerializeField]
    protected GameObject gameOverText;

    [SerializeField]
    protected GameObject gameMenu;
    [SerializeField]
    protected GameObject pauseMenu;
    [SerializeField]
    protected OptionsMenu optionsMenu;
    [SerializeField]
    protected GameObject endMenu;
    [SerializeField]
    protected GameObject beforeStartText;
    [SerializeField]
    protected UILivesController livesController;

    private void Start()
    {
        this.playerText.text = "Player: " + PlayerManager.Instance.PlayerName;
        DataStorage.Instance.ScoresChanged += OnUserScoresChanged;
        this.UpdateBestScore();
#if UNITY_WEBGL
        this.bestScoreText.gameObject.SetActive(false);
#endif
    }

    public void OnUserPointsChanged()
    {
        int userPoints = MainManager.Instance.CurrentUserPoints;
        scoreText.text = $"Score : {userPoints}";
    }

    public void OnGameStateChanged()
    {
        Cursor.visible = false;
        this.scoreText.gameObject.SetActive(true);
        this.levelText.gameObject.SetActive(true);
        this.gameMenu.SetActive(false);
        this.pauseMenu.SetActive(false);
        this.optionsMenu.gameObject.SetActive(false);
        this.beforeStartText.SetActive(false);
        this.gameOverText.SetActive(false);
        this.endMenu.SetActive(false);

        this.UpdateLevel();

        MainManager.GameState gameState = MainManager.Instance.CurrentGameState;
        switch (gameState)
        {
            case MainManager.GameState.Intro:
                Cursor.visible = true;
                this.scoreText.gameObject.SetActive(false);
                this.levelText.gameObject.SetActive(false);
                this.gameMenu.SetActive(true);
                break;
            case MainManager.GameState.Initial:
            case MainManager.GameState.BallLost:
                this.beforeStartText.SetActive(true);
                break;
            case MainManager.GameState.Playing:
                this.livesController.OnLivesChanged();
                break;
            case MainManager.GameState.GameOver:
                this.gameOverText.SetActive(true);
                break;
            case MainManager.GameState.GameComplete:
                Cursor.visible = true;
                this.endMenu.SetActive(true);
                break;
        }
        if (gameState == MainManager.GameState.Playing)
        {
            this.livesController.OnLivesChanged();
        }
    }

    public void OnGamePausedChanged()
    {
        bool gamePaused = MainManager.Instance.GamePaused;
        if (gamePaused)
        {
            Cursor.visible = true;
            this.pauseMenu.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            this.pauseMenu.SetActive(false);
            this.optionsMenu.gameObject.SetActive(false);
        }
    }

    public void OnUserLivesChanged()
    {
        this.livesController.OnLivesChanged();
    }

    public void OnUserScoresChanged()
    {
        this.UpdateBestScore();
    }

    public void PlayClicked()
    {
        StartCoroutine(PlayDelayed());
    }

    IEnumerator PlayDelayed()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Cursor.visible = false;
        MainManager.Instance.StartGame();
    }

    public void ResumeClicked()
    {
        MainManager.Instance.UnPauseGame();
    }

    public void OptionsClicked()
    {
        Cursor.visible = true;
        MainManager.GameState gameState = MainManager.Instance.CurrentGameState;
        if (gameState == MainManager.GameState.Intro)
        {
            this.gameMenu.SetActive(false);
        }
        else
        {
            this.pauseMenu.SetActive(false);
        }
        this.optionsMenu.gameObject.SetActive(true);
    }

    public void OptionsMenuBackClicked()
    {
        MainManager.GameState gameState = MainManager.Instance.CurrentGameState;
        if (gameState == MainManager.GameState.Intro)
        {
            this.gameMenu.SetActive(true);
        }
        else
        {
            this.pauseMenu.SetActive(true);
        }
        this.optionsMenu.gameObject.SetActive(false);
    }

    public void MainMenuClicked()
    {
        MainManager.Instance.MainMenu();
    }

    public void ExitGameClicked()
    {
        MainManager.Instance.ExitGame();
    }

    protected void UpdateBestScore()
    {
        if (this.bestScoreText == null)
            return;
        DataStorage.UserScore userScore = DataStorage.Instance.BestScore;
        this.bestScoreText.text = "Best score: ";
        if (userScore != null)
        {
            this.bestScoreText.text += userScore.userName + ": " + userScore.score;
        }
    }

    protected void UpdateLevel()
    {
        this.levelText.text = "Level: " + MainManager.Instance.CurrentLevel;
    }

}
