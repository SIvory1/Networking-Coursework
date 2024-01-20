using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValues : NetworkBehaviour
{

    public NetworkVariable<int> health = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    int maxHealth = 5;
    [SerializeField] TextMeshProUGUI healthText;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        health.Value = maxHealth;
        healthText.text = "" + health.Value;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0))
            // almost works, whoever takes dmg- on the other screen they are 1 health behind
            TakeDamage(1);
    }

    public void TakeDamage(int dmg)
    {
        health.Value -= dmg;

        UpdateHealthServerRPC();
        //call ui
        CheckForDeath();
    }

    [ClientRpc]
    void UpdateHealthClientRPC()
    {
        GameObject[] _healhText = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in _healhText)
        {
            obj.GetComponent<PlayerValues>().healthText.text = "" + (obj.GetComponent<PlayerValues>().health.Value);
        }
    }

    [ServerRpc]
    void UpdateHealthServerRPC()
    {
        UpdateHealthClientRPC();
    }

    // player cant trigger this themself 
    void CheckForDeath()
    {
        if (health.Value <= 0)
        {
            print("death con" + OwnerClientId);
            GameManager.instance.uiManager.GameOverUIServerRPC();
        } 
    }

    void OnGUI()
    {
        GameObject[] thePlayers2 = GameObject.FindGameObjectsWithTag("Player");
        int x = 0;
        foreach (GameObject respawn in thePlayers2)
        {
            GUI.Label(new Rect(10, 60 + (15 * x), 300, 20), "PlayerID " + respawn.GetComponent<NetworkObject>().NetworkObjectId + " has the score of " + respawn.GetComponent<PlayerValues>().health.Value);
            x++;
        }
    }
}