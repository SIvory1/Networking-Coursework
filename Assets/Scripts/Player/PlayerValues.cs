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
        if (healthSlider.value != _health)
        {
        healthSlider.value = _health;
        }
    }

    public void TakeDamage(int _dmg)
    {
        UpdateHealthServerRPC(_dmg);
        CheckForDeathServerRpc(health);
    }

    [ServerRpc]
    void UpdateHealthServerRPC(int _dmg)
    {
        health -= _dmg;
    }

    [ServerRpc]
    void CheckForDeathServerRpc(int _health)
    {
        CheckForDeathClientRpc(_health);
    }

    bool once = false;

    [ClientRpc]
    void CheckForDeathClientRpc(int _health)
    {
        if (_health == 0 && !once)
        {
         //   print(_health);
            GameManager.instance.uiManager.GameOver();
            once = true;
        }
    }

    //void OnGUI()
    //{
    //    GameObject[] thePlayers2 = GameObject.FindGameObjectsWithTag("Player");
    //    int x = 0;
    //    foreach (GameObject respawn in thePlayers2)
    //    {
    //        GUI.Label(new Rect(10, 60 + (15 * x), 300, 20), "PlayerID " + respawn.GetComponent<NetworkObject>().NetworkObjectId + " has the score of " + respawn.GetComponent<PlayerValues>().health);
    //        x++;
    //    }
    //}
}