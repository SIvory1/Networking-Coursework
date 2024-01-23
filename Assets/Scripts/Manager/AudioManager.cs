using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour 
{

    [SerializeField] private AudioClip zapNoise;
    [SerializeField] private AudioClip deathNoise;


    [SerializeField] GameObject audioObject;
   
    [ServerRpc(RequireOwnership = false)]
    public void PlayZapServerRPC()
    {
        PlayZapClientRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayDeathServerRPC()
    {
        PlayDeathClientRPC();
    }

    [ClientRpc]
    public void PlayZapClientRPC()
    {
        GameObject audio = Instantiate(audioObject, Vector3.zero, Quaternion.identity);

        audio.GetComponent<AudioSource>().PlayOneShot(zapNoise);
        StartCoroutine(DeleteAudio(audio));
    }

    [ClientRpc]
    public void PlayDeathClientRPC()
    {
        GameObject audio = Instantiate(audioObject, Vector3.zero, Quaternion.identity);

        audio.GetComponent<AudioSource>().PlayOneShot(deathNoise);
        StartCoroutine(DeleteAudio(audio));
    }


    private float deleteAudio = 10;

    IEnumerator DeleteAudio(GameObject audio)
    {
        yield return new WaitForSeconds(deleteAudio);
        Destroy(audio);
    }
}
