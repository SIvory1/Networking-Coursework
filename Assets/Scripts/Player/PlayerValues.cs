using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerValues : NetworkBehaviour
{

    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    int health;
    int maxHealth = 5;
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

        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0))
            TakeDamage(1);
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
         CheckForDeathServerRpc(health);
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
            print("death con" + OwnerClientId);
            GameManager.instance.uiManager.GameOverUIClientRPC();
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