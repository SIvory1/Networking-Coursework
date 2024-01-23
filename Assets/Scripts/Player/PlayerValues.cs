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
        if (IsServer) CheckForDeathServerRpc(health);
    }

    [ClientRpc]
    void UpdateHealthClientRPC(int _health)
    {
           healthSlider.value = _health;
    }

    [ServerRpc]
    void UpdateHealthServerRPC(int _dmg)
    {
        health -= _dmg;

        UpdateHealthClientRPC(health);
    }

    [ServerRpc]
    void CheckForDeathServerRpc(int _health)
    {
        CheckForDeathClientRpc(_health);
    }

    [ClientRpc]
    void CheckForDeathClientRpc(int _health)
    {
        if (_health <= 0)
        {
            print("inhere");
            GameManager.instance.uiManager.GameOverUIServerRPC();
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