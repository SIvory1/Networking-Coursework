using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    public UIManager uiManager { get; private set; }
    public AudioManager audioManager { get; private set; }

    public float enemyCounter = 2;

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

    public void CheckEnemyCount()
    {
        enemyCounter--;

        if (enemyCounter <= 0)
        {
            uiManager.GameOver();
        }
    }

}
