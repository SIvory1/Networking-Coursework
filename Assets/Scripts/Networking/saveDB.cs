using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public struct PlayerData
{
    public ulong playerID;
    public int time;
    public int health;
}

public class saveDB : NetworkBehaviour
{

    Dictionary<string, int> playersScoresDictionary = new Dictionary<string, int>();

    public void PostData()
    {
        // need is host for client to see ui 
        if (IsServer)
        {
            playersScoresDictionary.Add(UIManager.playerName, (int)GameManager.instance.uiManager.time);
        }
        // tske too long
        sendDataToServerScript();
    }

    void sendDataToServerScript()
    {
        StartCoroutine(toDatabase());
    }

    IEnumerator toDatabase()
    {
        int xcount = 0;
        foreach (KeyValuePair<string, int> entry in playersScoresDictionary)
        {
            WWWForm form = new WWWForm();
            form.AddField("clientid", entry.Key);
            form.AddField("score", entry.Value);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/unityMultiplayerLeaderboard/leaderboard.php", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                xcount++;

                Debug.Log(www.downloadHandler.text);
            }

            www.Dispose();
        }
    }
}
