using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{

    [System.Serializable]
    public class StoredData
    {
        public List<UserScore> bestScores = new List<UserScore>();
    }

    [System.Serializable]
    public class UserScore
    {
        public string userName;
        public int score;
    }

    public static DataStorage Instance { get; private set; }

    public UserScore[] BestScores { get => this.storedData.bestScores.ToArray(); }
    public UserScore BestScore { get => this.storedData.bestScores.Count > 0 ? this.storedData.bestScores[0] : null; }

    protected StoredData storedData = new StoredData();

    protected static int maxBestScoresCount = 10;
    protected string storedDataFilePath;

    public event System.Action ScoresChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        this.storedDataFilePath = Application.persistentDataPath + "/savefile.json";
        this.Load();
    }

    public void AddUserScore(string userName, int score)
    {
        List<UserScore> bestScores = this.storedData.bestScores;
        UserScore userScore = bestScores.Find(s => s.userName.Equals(userName));
        if (userScore != null && userScore.score > score)
            return;
        if (userScore == null)
        {
            userScore = new UserScore();

            bestScores.Add(userScore);
        }
        userScore.userName = userName;
        userScore.score = score;
        this.processBestScores();
        this.ScoresChanged?.Invoke();
        this.Save();
    }

    protected void processBestScores()
    {
        List<UserScore> bestScores = this.storedData.bestScores;
        bestScores.Sort((s1, s2) => s2.score.CompareTo(s1.score));
        while (bestScores.Count > maxBestScoresCount)
        {
            bestScores.RemoveRange(maxBestScoresCount, bestScores.Count - maxBestScoresCount);
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this.storedData);
        System.IO.File.WriteAllText(this.storedDataFilePath, json);
    }

    public void Load()
    {
        if (!System.IO.File.Exists(this.storedDataFilePath))
            return;
        string json = System.IO.File.ReadAllText(this.storedDataFilePath);
        this.storedData = JsonUtility.FromJson<StoredData>(json);
        this.processBestScores();
        this.ScoresChanged?.Invoke();
    }

}
