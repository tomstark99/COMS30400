using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun
{
    [SerializeField]
    private AudioSource gunShot;

    public void GunShot()
    {
        photonView.RPC("PlayGunShot", RpcTarget.All);
    }

    [PunRPC]
    void PlayGunShot()
    {
        gunShot.PlayOneShot(gunShot.clip);
    }
}
