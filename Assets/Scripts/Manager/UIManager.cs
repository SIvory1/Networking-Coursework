using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float time;
    [SerializeField] GameObject gameOverUI;

    private void Start()
    {
        UpdateTimerUI(time);
    }

    private void Update()
    {
        UpdateTimeClientRpc(time -= Time.deltaTime);
    }

    [ClientRpc]
    void UpdateTimeClientRpc(float _adjustedTime)
    {
        if (time <= 0)
        {
            print("its over john... we did it");
        }
        else
        {
            time = _adjustedTime;
            UpdateTimerUI(time);
        }
    }

    void UpdateTimerUI(float _time)
    {
        float min = Mathf.FloorToInt(_time / 60);
        float sec = Mathf.FloorToInt(_time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    [ClientRpc]
    public void GameOverUIClientRPC()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReplayGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    // quits client, if host shuts down game 
    public void DisconnectPlayers()
    {
        NetworkManager.Singleton.Shutdown();
    }

    [ServerRpc]
    void GameOverUIServerRPC()
    {
        GameOverUIClientRPC();
    }
}
