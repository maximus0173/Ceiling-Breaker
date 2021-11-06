using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager Instance { get; private set; }

    public string PlayerName { get => this.playerName; }
    protected string playerName = "Player 1";

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

    public void SetPlayerName(string playerName)
    {
        if (playerName == null || playerName.Trim().Length == 0)
            return;
        this.playerName = playerName.Trim();
        if (this.playerName.Length > 16)
        {
            this.playerName = this.playerName.Substring(0, 16);
        }
    }

}
