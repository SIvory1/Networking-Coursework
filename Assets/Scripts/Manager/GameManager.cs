using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager instance;


    public UIManager uiManager { get; private set; }
    public AudioManager audioManager { get; private set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance != this)
        {
            Destroy(gameObject);
        }
        InitSystems();
    }       


    private void InitSystems()
    {
        uiManager = GetComponent<UIManager>();
        audioManager = GetComponent<AudioManager>();
    }

    private void Update()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    GameObject[] players;
    private void OnClientConnectedCallback(ulong clientId)
    {
       // players = GameObject.FindGameObjectsWithTag("Player");
        print("jogn");
    }
}
