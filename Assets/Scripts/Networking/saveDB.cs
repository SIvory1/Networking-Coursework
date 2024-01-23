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
        playersScoresDictionary.Add(UIManager.playerName, (int)GameManager.instance.uiManager.time);
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
            if (xcount == playersScoresDictionary.Count)
            {
               // changeSceneServerRpc();
            }

            www.Dispose();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void changeSceneServerRpc()
    {
        var status = NetworkManager.SceneManager.LoadScene("leaderboard", LoadSceneMode.Single);

        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load {"leaderboard"} " +
                  $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }

}
