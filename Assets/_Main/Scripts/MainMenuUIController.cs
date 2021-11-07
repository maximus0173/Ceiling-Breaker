using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{

    [SerializeField]
    protected TMP_InputField playerNameInputField;

    [SerializeField]
    protected GameObject gameMenu;

    [SerializeField]
    protected OptionsMenu optionsMenu;

    [SerializeField]
    protected BestScoresMenu bestScoresMenu;

    private void Start()
    {
        this.playerNameInputField.text = PlayerManager.Instance.PlayerName;
#if UNITY_WEBGL
        return;
#endif
    }

    public void GameClicked()
    {
        string playerName = this.playerNameInputField.text.Trim();
        if (playerName.Length > 0)
        {
            PlayerManager.Instance.SetPlayerName(playerName);
            this.playerNameInputField.text = PlayerManager.Instance.PlayerName;
            StartCoroutine(GameDelayed());
        }
        else
        {
            StartCoroutine(PlayerNameWarning());
        }
    }

    IEnumerator GameDelayed()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(1);
    }

    IEnumerator PlayerNameWarning()
    {
        Image image = this.playerNameInputField.GetComponent<Image>();
        image.color = Color.red;
        yield return new WaitForSecondsRealtime(2f);
        image.color = Color.white;
    }

    public void OptionsClicked()
    {
        this.gameMenu.SetActive(false);
        this.optionsMenu.gameObject.SetActive(true);
    }

    public void OptionsMenuBackClicked()
    {
        this.optionsMenu.gameObject.SetActive(false);
        this.gameMenu.SetActive(true);
    }

    public void BestScoresClicked()
    {
#if UNITY_WEBGL
        return;
#endif
        this.gameMenu.SetActive(false);
        this.bestScoresMenu.gameObject.SetActive(true);
    }

    public void BestScoresMenuBackClicked()
    {
        this.bestScoresMenu.gameObject.SetActive(false);
        this.gameMenu.SetActive(true);
    }

    public void QuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_WEBGL)
        return;
#else
        Application.Quit();
#endif
    }

}
