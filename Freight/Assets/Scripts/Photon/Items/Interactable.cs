using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary><c>Interactable</c> is the base class for anything which can be
/// interacted with. It add the funcitons for glowing. These can then be called
/// in <c>ItemInteract</c>. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PhotonView))]
public abstract class Interactable : MonoBehaviour
{
    protected PhotonView view;
    public virtual void PrimaryInteractionOff(Character character) {}

    public virtual void PrimaryInteraction(Character character) {}
    
    public virtual void Start() 
    {
        view = GetComponent<PhotonView>();
    }
}
