using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField]
    protected Text scoreText;
    [SerializeField]
    protected GameObject gameMenu;
    [SerializeField]
    protected GameObject pauseMenu;
    [SerializeField]
    protected OptionsMenu optionsMenu;
    [SerializeField]
    protected GameObject beforeStartText;
    [SerializeField]
    protected GameObject gameOverText;
    [SerializeField]
    protected UILivesController livesController;

    public void OnUserPointsChanged()
    {
        int userPoints = MainManager.Instance.CurrentUserPoints;
        scoreText.text = $"Score : {userPoints}";
    }

    public void OnGameStateChanged()
    {
        this.gameMenu.SetActive(false);
        this.pauseMenu.SetActive(false);
        this.optionsMenu.gameObject.SetActive(false);
        this.beforeStartText.SetActive(false);
        this.gameOverText.SetActive(false);

        MainManager.GameState gameState = MainManager.Instance.CurrentGameState;
        switch (gameState)
        {
            case MainManager.GameState.Intro:
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
            this.pauseMenu.SetActive(true);
        }
        else
        {
            this.pauseMenu.SetActive(false);
            this.optionsMenu.gameObject.SetActive(false);
        }
    }

    public void OnUserLivesChanged()
    {
        this.livesController.OnLivesChanged();
    }

    public void PlayClicked()
    {
        StartCoroutine(PlayDelayed());
    }

    IEnumerator PlayDelayed()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        MainManager.Instance.StartGame();
    }

    public void ResumeClicked()
    {
        MainManager.Instance.UnPauseGame();
    }

    public void OptionsClicked()
    {
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

    }

    public void ExitGameClicked()
    {
        MainManager.Instance.ExitGame();
    }

}
