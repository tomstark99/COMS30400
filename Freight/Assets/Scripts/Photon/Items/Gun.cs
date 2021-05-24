using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

/*
    Class that applies canvas and gunshot sound
*/
public class Gun : MonoBehaviourPun
{
    [SerializeField]
    private AudioSource gunShot;

    [SerializeField]
    private AudioSource emptyClip;

    [SerializeField]
    private int ammo;

    [SerializeField]
    private TextMeshProUGUI ammoUI;

    public int Ammo
    {
        get { return ammo; }
    }

    void Start()
    {
        // sets the UI ammo text
        ammoUI.text = "Ammo: " + ammo.ToString();
    }

    public void EmptyGunShot()
    {
        photonView.RPC(nameof(PlayEmptyClip), RpcTarget.All);
    }

    // RPC call to tell all players to play empty gunshot sound 
    [PunRPC]
    void PlayEmptyClip()
    {
        emptyClip.PlayOneShot(emptyClip.clip);
    }

    public void GunShot()
    {
        photonView.RPC(nameof(PlayGunShot), RpcTarget.All);
    }

    // RPC call to tell all players to play gunshot sound 
    [PunRPC]
    void PlayGunShot()
    {
        // plays gunshot sound and updates ammo count with decreased ammo
        gunShot.PlayOneShot(gunShot.clip);
        ammo -= 1;
        ammoUI.text = "Ammo: " + ammo.ToString();
    }
}
