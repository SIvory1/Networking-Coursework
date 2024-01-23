using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        // print(playerName);
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
        GameObject databaseObj = GameObject.FindGameObjectWithTag("Database");

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
        //  Time.timeScale = 1;
        Destroy(networkObject);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    // quits client, if host shuts down game 
    public void DisconnectPlayers()
    {
        NetworkManager.Singleton.Shutdown();
    }

   [ServerRpc(RequireOwnership = false)]
   public void GameOverUIServerRPC()
   {
        GameOverUIClientRPC();
   }
}
