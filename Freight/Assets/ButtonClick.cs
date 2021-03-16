﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class ButtonClick : MonoBehaviourPun
{
    public AudioSource source;
    public AudioClip hover;
    public AudioClip click;

    //function for hover button sound
    public void HoverSound()
    {
        source.PlayOneShot(hover);
    }
    //function for click button sound
    public void ClickSound()
    {
        source.PlayOneShot(click);
    }
    
}
