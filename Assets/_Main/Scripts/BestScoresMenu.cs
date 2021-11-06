using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BestScoresMenu : MonoBehaviour
{

    public UnityEvent backEvent;
    public TMP_Text namesText;
    public TMP_Text scoresText;

    private void OnEnable()
    {
        this.UpdateScores();
    }

    public void BackClicked()
    {
        this.backEvent.Invoke();
    }

    protected void UpdateScores()
    {
        List<DataStorage.UserScore> bestScores = new List<DataStorage.UserScore>(DataStorage.Instance.BestScores);
        List<string> names = bestScores.Select(s => s.userName).ToList();
        List<string> scores = bestScores.Select(s => s.score.ToString()).ToList();
        this.namesText.text = string.Join("\n", names);
        this.scoresText.text = string.Join("\n", scores);
    }

}
