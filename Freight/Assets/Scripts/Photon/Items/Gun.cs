using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

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
        ammoUI.text = "Ammo: " + ammo.ToString();
    }

    public void EmptyGunShot()
    {
        photonView.RPC("PlayEmptyClip", RpcTarget.All);
        Debug.Log(ammo);
    }

    [PunRPC]
    void PlayEmptyClip()
    {
        emptyClip.PlayOneShot(emptyClip.clip);
    }

    public void GunShot()
    {
        photonView.RPC("PlayGunShot", RpcTarget.All);
    }

    [PunRPC]
    void PlayGunShot()
    {
        gunShot.PlayOneShot(gunShot.clip);
        ammo -= 1;
        ammoUI.text = "Ammo: " + ammo.ToString();
    }
}
