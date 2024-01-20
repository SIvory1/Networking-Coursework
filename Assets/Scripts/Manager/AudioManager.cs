using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour 
{

    [SerializeField] private AudioClip zapNoise;

    [SerializeField] GameObject audioObject;
   
    [ServerRpc(RequireOwnership = false)]
    public void PlayZapServerRPC()
    {
        PlayZapClientRPC();
    }

    [ClientRpc]
    public void PlayZapClientRPC()
    {
        GameObject audio = Instantiate(audioObject, Vector3.zero, Quaternion.identity);
       // audio.GetComponent<NetworkObject>().Spawn(true);

        audio.GetComponent<AudioSource>().PlayOneShot(zapNoise);
        StartCoroutine(DeleteAudio(audio));
    }

    private float deleteAudio = 10;

    IEnumerator DeleteAudio(GameObject audio)
    {
        yield return new WaitForSeconds(deleteAudio);
        Destroy(audio);
    }
}
