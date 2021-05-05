using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class ButtonClick : MonoBehaviourPun
{
    public AudioSource source;
    public AudioClip hover;


    public void HoverSound()
    {
        source.PlayOneShot(hover);
    }

   
}
