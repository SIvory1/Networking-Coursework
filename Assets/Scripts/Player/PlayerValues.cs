using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerValues : NetworkBehaviour
{

    public NetworkVariable<int> health = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    int maxHealth = 5;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Slider healthSlider;

    public override void OnNetworkSpawn()
    { 
        if (IsOwner)
        {
            health.Value = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health.Value;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0))
            // almost works, whoever takes dmg- on the other screen they are 1 health behind
            TakeDamage(1);
    }

    public void TakeDamage(int _dmg)
    {
         UpdateHealthServerRPC(_dmg);
      //  StartCoroutine(CheckHealth(_dmg));  
       // CheckForDeath();
    }

    IEnumerator CheckHealth(int _dmg)
    {
        yield return new WaitForSeconds(0.25f);
        UpdateHealthServerRPC(_dmg);
    }

    [ClientRpc]
    void UpdateHealthClientRPC(int _dmg)
    {
        if (IsOwner)
        {
            health.Value -= _dmg;
            healthSlider.value = health.Value;

        }
        // GameObject[] _healhText = GameObject.FindGameObjectsWithTag("Player");
        // foreach (GameObject obj in _healhText)
        // {
        // }
    }

    [ServerRpc]
    void UpdateHealthServerRPC(int _dmg)
    {
        UpdateHealthClientRPC(_dmg);
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