using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField]
    protected Text scoreText;
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
        MainManager.GameState gameState = MainManager.Instance.CurrentGameState;
        switch (gameState)
        {
            case MainManager.GameState.Initial:
                this.beforeStartText.SetActive(true);
                break;
            case MainManager.GameState.Playing:
                this.beforeStartText.SetActive(false);
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

    public void OnUserLivesChanged()
    {
        this.livesController.OnLivesChanged();
    }

}
