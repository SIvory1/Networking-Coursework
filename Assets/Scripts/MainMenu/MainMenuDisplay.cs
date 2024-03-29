using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuDisplay : MonoBehaviour
{

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
    public void StartServer()
    { 
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
