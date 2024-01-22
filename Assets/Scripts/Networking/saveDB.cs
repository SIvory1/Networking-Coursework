using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class saveDB : NetworkBehaviour
{

    bool XIsPressed;

    Dictionary<ulong, int> playersScoresDictionary = new Dictionary<ulong, int>();

    void Start()
    {
        XIsPressed = false;

        playersScoresDictionary.Add(1, 15);
        playersScoresDictionary.Add(2, 25);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && !XIsPressed)
        {
            //to make sure X is only pressed once
            XIsPressed = true;
            sendDataToServerScript();
        }
    }

    void sendDataToServerScript()
    {
        StartCoroutine(toDatabase());
    }

    IEnumerator toDatabase()
    {
        int xcount = 0;
        foreach (KeyValuePair<ulong, int> entry in playersScoresDictionary)
        {
            WWWForm form = new WWWForm();
            form.AddField("clientid", (int)entry.Key);
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
                changeSceneServerRpc();
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
