using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
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
      //  if (IsClient)
      //  {
           // NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
     //   }
    }


    // in lobby system do it after when everyone joins easy peasy
    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            //    for (int i = 0; i < enemies.Length; i++)
            //    {
            //        enemies[i].GetComponent<BasicEnemy>()._player = GameObject.FindGameObjectsWithTag("Player");
            //        enemies[i].GetComponent<BasicEnemy>().distanceFromPlayer = new float[enemies[i].GetComponent<BasicEnemy>()._player.Length];
            //    }
            print("john");
            
        }

    }


    void OnPlayerConnected()
    {
        print("ronbald");

    }

}
