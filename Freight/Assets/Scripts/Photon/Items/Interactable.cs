using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
public abstract class Interactable : MonoBehaviourPun
{
    protected PhotonView view;
    public virtual void PrimaryInteractionOff(Character character) {}

    public virtual void PrimaryInteraction(Character character) {}
    
    public virtual void Start() 
    {
        view = GetComponent<PhotonView>();
    }
}
