using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpotlightSounds : MonoBehaviourPun
{

    private bool played = false;

    [PunRPC]
    void PlaySoundRPC()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player.GetPhotonView().IsMine)
            {
                player.GetComponent<PlayerAudioClips>().SpottedByLights();
                Destroy(this);
            }
        }
    }
         
    public void PlayDetectedSound()
    {
        if (!played)
        {
            played = true;
            photonView.RPC(nameof(PlaySoundRPC), RpcTarget.All);
            
        }
        
    }
}
