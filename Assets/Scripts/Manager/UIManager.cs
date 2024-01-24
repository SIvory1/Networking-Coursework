using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] public float time;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] public GameObject LeaderBoardObject;
    public TextMeshProUGUI inputBox;
    public static string playerName;

    private void Start()
    {
        if (timerText != null)
        {
            UpdateTimerUI(time);
        }
    }

    private void Update()
    {
         if (timerText != null)
         {
              UpdateTimeClientRpc(time -= Time.deltaTime);
         }

        if (inputBox != null)
        {
            playerName = inputBox.text.ToString();
        }
    }

    [ClientRpc]
    void UpdateTimeClientRpc(float _adjustedTime)
    {
        if (time <= 0)
        {
            print("its over john... we did it");
            GameOver();
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

    public void GameOver()
    {     
        GameObject databaseObj = GameObject.FindGameObjectWithTag("Database");
        // host time doesnt freeze so he gets stuck in here
        databaseObj.GetComponent<saveDB>().PostData();
        StartCoroutine(databaseObj.GetComponent<ReadFromLeaderboard>().ReadData());

        gameOverUI.SetActive(true);

        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReplayGame()
    {
        GameObject networkObject = GameObject.FindGameObjectWithTag("NetworkManager");
        Destroy(networkObject); 
       // NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("MainMenu");
    }

    // quits client, if host shuts down game 
    public void DisconnectPlayers()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
