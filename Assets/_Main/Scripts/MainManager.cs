using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;
using Sirenix.OdinInspector;

public class MainManager : MonoBehaviour
{

    public enum GameState
    {
        Intro,
        Initial,
        Playing,
        BallLost,
        GameOver,
        GameComplete
    }

    public static MainManager Instance { get; private set; }

    [SerializeField]
    protected int lives = 3;

    [SerializeField]
    protected GameObject ballPrefab;
    protected Ball ball;
    protected Rigidbody ballRb;

    [SerializeField]
    protected GameObject paddlePrefab;
    protected Paddle paddle;

    [SerializeField]
    protected Camera mainCamera;

    [SerializeField]
    protected CinemachineBlendListCamera introVirtCamera;

    [SerializeField]
    [ListDrawerSettings(NumberOfItemsPerPage = 10)]
    protected List<LevelManager> levels = new List<LevelManager>();

    public GameState CurrentGameState { get => this.gameState; }
    public int CurrentUserPoints { get => this.userPoints; }
    public bool GamePaused { get => this.isGamePaused; }

    public int CurrentLives { get => this.lives; }

    public int CurrentLevel { get => this.levelIndex + 1; }

    protected GameState gameState = GameState.Intro;
    protected int userPoints;
    [SerializeField]
    protected int levelIndex = 0;
    protected bool isGamePaused = false;

    [SerializeField]
    protected int healingCount = 6;

    [SerializeField]
    protected AudioSource audioMusic1;
    [SerializeField]
    protected AudioSource audioMusic2;

    protected float playingMusicBlend = 1f;
    protected int playingMusic = 1;

    public UnityEvent GameStateChanged;
    public UnityEvent UserPointsChanged;
    public UnityEvent UserLivesChanged;
    public UnityEvent GamePausedChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        this.ChangeGameState(GameState.Intro);
        StartCoroutine(updateMusic());
        this.AddHealingBricks();
    }

    private void Update()
    {
        if (this.gameState == GameState.Initial || this.gameState == GameState.BallLost)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.StartBall();
            }
        }
        else if (this.gameState == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ExitGame();
            }
        }
        
        if (this.gameState != GameState.Intro && this.gameState != GameState.GameComplete)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                if (!this.isGamePaused)
                {
                    PauseGame();
                }
                else
                {
                    UnPauseGame();
                }
            }
        }
    }

    public void StartGame()
    {
        if (this.gameState != GameState.Intro)
            return;
        this.introVirtCamera.gameObject.SetActive(false);
        this.ChangeGameState(GameState.Initial);
        foreach (LevelManager level in this.levels)
        {
            level.InitLevel();
            level.OnLevelComplete += HandleLevelComplete;
        }
        if (this.levels.Count > this.levelIndex)
        {
            LevelManager level = this.levels[this.levelIndex];
            level.StartLevel();
            this.CreatePaddle(level.BasePosition);
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    protected void StartBall()
    {
        Vector3 ballPosition = this.paddle.transform.position + Vector3.up * 1f;
        GameObject ballGameObject = Instantiate(this.ballPrefab, ballPosition, Quaternion.identity);
        this.ball = ballGameObject.GetComponent<Ball>();
        this.ballRb = this.ball.GetComponent<Rigidbody>();

        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDir = new Vector3(randomDirection, 1, 0);
        forceDir.Normalize();

        this.ball.transform.SetParent(null);
        this.ballRb.AddForce(forceDir * this.ball.MaxSpeed, ForceMode.VelocityChange);

        this.ChangeGameState(GameState.Playing);
    }

    public void AddPoint(int point)
    {
        this.userPoints += point;
        this.UserPointsChanged?.Invoke();
    }

    public void BallLost()
    {
        if (this.lives - 1 >= 0)
        {
            this.lives--;
            this.UserLivesChanged?.Invoke();
            this.ChangeGameState(GameState.BallLost);
        }
        if (this.lives <= 0)
        {
            this.GameOver();
        }
    }

    public void GameOver()
    {
        string playerName = PlayerManager.Instance.PlayerName;
        DataStorage.Instance.AddUserScore(playerName, this.CurrentUserPoints);
        this.ChangeGameState(GameState.GameOver);
    }

    protected void ChangeGameState(GameState newGameState)
    {
        this.gameState = newGameState;
        this.GameStateChanged?.Invoke();
        switch (this.gameState)
        {
            case GameState.Playing:
                this.PlayMusic(2);
                break;
            default:
                this.PlayMusic(1);
                break;
        }
    }

    public void SetLevels(ICollection<LevelManager> levels)
    {
        this.levels.Clear();
        foreach (LevelManager level in levels)
        {
            this.levels.Add(level);
        }
    }

    public void PauseGame()
    {
        if (this.gameState == GameState.Intro || this.isGamePaused)
            return;
        Time.timeScale = 0f;
        this.isGamePaused = true;
        this.GamePausedChanged?.Invoke();
    }

    public void UnPauseGame()
    {
        if (this.gameState == GameState.Intro || !this.isGamePaused)
            return;
        Time.timeScale = 1f;
        this.isGamePaused = false;
        this.GamePausedChanged?.Invoke();
    }

    protected void HandleLevelComplete(LevelManager completedLevel)
    {
        Destroy(this.ball.gameObject);
        this.ballRb = null;
        this.paddle.DestroyPaddle();
        StartCoroutine(LevelCompleteTask(completedLevel));
    }

    IEnumerator LevelCompleteTask(LevelManager completedLevel)
    {
        yield return new WaitForSecondsRealtime(2f);
        completedLevel.EndLevel();
        int levelIndex = this.levels.FindIndex(o => o == completedLevel);
        levelIndex++;
        if (levelIndex >= 0 && levelIndex < this.levels.Count)
        {
            this.levelIndex = levelIndex;
            LevelManager nextLevel = this.levels[levelIndex];
            nextLevel.StartLevel();

            this.CreatePaddle(nextLevel.BasePosition);
            this.ChangeGameState(GameState.Initial);
        }
        else
        {
            this.ChangeGameState(GameState.GameComplete);
        }
    }

    protected void CreatePaddle(Vector3 position)
    {
        GameObject go = Instantiate(this.paddlePrefab.gameObject);
        go.transform.position += position;
        go.transform.rotation = Quaternion.identity;
        this.paddle = go.GetComponent<Paddle>();
    }

    protected void PlayMusic(int number)
    {
        this.playingMusic = number;
    }

    IEnumerator updateMusic()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            if ((float)this.playingMusic > this.playingMusicBlend)
            {
                this.playingMusicBlend += 0.1f;
            }
            else if ((float)this.playingMusic < this.playingMusicBlend)
            {
                this.playingMusicBlend -= 0.1f;
            }
            float musicVolume1 = Mathf.Clamp(2 - this.playingMusicBlend, 0f, 1f);
            float musicVolume2 = Mathf.Clamp(this.playingMusicBlend - 1, 0f, 1f);
            this.audioMusic1.volume = musicVolume1;
            this.audioMusic2.volume = musicVolume2;
            if (musicVolume1 > 0f)
            {
                if (!this.audioMusic1.isPlaying)
                {
                    this.audioMusic1.Play();
                }
            }
            else
            {
                this.audioMusic1.Pause();
            }
            if (musicVolume2 > 0f)
            {
                if (!this.audioMusic2.isPlaying)
                {
                    this.audioMusic2.Play();
                }
            }
            else
            {
                this.audioMusic2.Pause();
            }
        }
    }

    public void AddLife()
    {
        this.lives++;
        this.UserLivesChanged?.Invoke();
    }

    protected void AddHealingBricks()
    {
        for (int i = 0; i < this.healingCount; i++)
        {
            int levelIndex = Random.Range(1, this.levels.Count - 1);
            this.levels[levelIndex].AddHealing();
        }

    }

}
