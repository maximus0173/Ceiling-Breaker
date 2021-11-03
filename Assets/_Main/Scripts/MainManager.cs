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
        GameOver
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

    public int CurrentLives { get => this.lives; }

    protected GameState gameState = GameState.Intro;

    protected int userPoints;

    protected int levelIndex = 0;

    public UnityEvent GameStateChanged;
    public UnityEvent UserPointsChanged;
    public UnityEvent UserLivesChanged;

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
        this.ChangeGameState(GameState.Intro);
    }

    private void Update()
    {
        if (this.gameState == GameState.Intro)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.StartGame();
            }
        }
        else if (this.gameState == GameState.Initial || this.gameState == GameState.BallLost)
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    protected void StartGame()
    {
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
        this.UserPointsChanged.Invoke();
    }

    public void BallLost()
    {
        if (this.lives - 1 >= 0)
        {
            this.lives--;
            this.UserLivesChanged.Invoke();
            this.ChangeGameState(GameState.BallLost);
        }
        if (this.lives <= 0)
        {
            this.GameOver();
        }
    }

    public void GameOver()
    {
        this.ChangeGameState(GameState.GameOver);
    }

    protected void ChangeGameState(GameState newGameState)
    {
        this.gameState = newGameState;
        this.GameStateChanged.Invoke();
    }

    public void SetLevels(ICollection<LevelManager> levels)
    {
        this.levels.Clear();
        foreach (LevelManager level in levels)
        {
            this.levels.Add(level);
        }
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
        yield return new WaitForSeconds(5f);
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
    }

    protected void CreatePaddle(Vector3 position)
    {
        GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(this.paddlePrefab.gameObject) as GameObject;
        go.transform.position += position;
        go.transform.rotation = Quaternion.identity;
        this.paddle = go.GetComponent<Paddle>();
    }

}
