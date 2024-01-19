using System.Collections;
using System.Collections.Generic;
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
}
