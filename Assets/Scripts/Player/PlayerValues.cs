using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerValues : NetworkBehaviour
{

    int health;
    [SerializeField] int maxHealth;
    [SerializeField] Slider healthSlider;

    public override void OnNetworkSpawn()
    { 
            health = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;  
    }

    private void Update()
    {
        UpdateHealthClientRpc(health);
        CheckForDeathClientRpc(health);
    }

    // ineffiecent but updates for late joining
    [ClientRpc]
    void UpdateHealthClientRpc(int _health)
    { 
           healthSlider.value = _health;
    }

    public void TakeDamage(int _dmg)
    {
        UpdateHealthServerRPC(_dmg);
    } 

    [ServerRpc]
    void UpdateHealthServerRPC(int _dmg)
    {
        health -= _dmg;
    }

    bool once = false;

    [ClientRpc]
    void CheckForDeathClientRpc(int _health)
    {
        if (_health == 0 && !once)
        {
            GameManager.instance.uiManager.GameOver();
            once = true;
        }
    }
}