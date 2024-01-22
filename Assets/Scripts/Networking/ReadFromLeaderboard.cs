using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadFromLeaderboard : MonoBehaviour
{
    string[] theResultString;
    bool getDataSuccess;

    IEnumerator Start()
    {
        //the delimiter basically to parse the data which in this case we are going to use ‘&’ to distinguish new row / data record
        char[] delimiterChars = { '&' };

        //connect to the php script
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/unityMultiplayerLeaderboard/leaderboardend.php");
        yield return www.SendWebRequest();


        //check if the result is correct. If the result is back, split / parse the information by the delimiter
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            getDataSuccess = false;
        }
        else
        {
            theResultString = www.downloadHandler.text.Split(delimiterChars);
            getDataSuccess = true;
        }
        www.Dispose();
    }

    void OnGUI()
    {
        //show the parsed information to screen
        if (getDataSuccess)
        {
            for (int i = 0; i < theResultString.Length - 1; i++)
            {
                GUI.Label(new Rect(10, 60 + (15 * i + 1), 300, 20), theResultString[i]);
            }
        }
    }
}
