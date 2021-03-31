using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FrostweepGames.Plugins.Native;
using FrostweepGames.WebGLPUNVoice;
using System.Linq;
using System;

public class ProximityVoice : MonoBehaviourPun
{
    public VoiceChatConnector voice;

    private GameObject otherPlayer = null;
    private AudioSource otherPlayerSource;

    public float minDistance = 1f;
    public float maxDistance = 25f;
    private readonly float maxVolume = 1f;
    private readonly float minVolume = 0.01f;

    private float a;
    private float b;

    private readonly float updateFrequency = 0.1f;

    void Start()
    {
        if (!photonView.IsMine) return;

        voice.OnStatusConnected += UpdateOtherPlayer;
        otherPlayerSource = voice.foreignAudioSource;

        b = (minVolume - maxVolume) / (1 / Mathf.Sqrt(maxDistance) - 1 / Mathf.Sqrt(minDistance));
        a = maxVolume - b / Mathf.Sqrt(minDistance);

        InvokeRepeating(nameof(UpdateVolume), 0, updateFrequency); // call it 1 / updateFrequency times per second
    }

    private float VolumeValue(float distance)
    {
        if (distance < minDistance) return 1f; //Max volume
        if (distance > maxDistance) return 0f; //Min volume
        return a + b / Mathf.Sqrt(distance); // maxVolume in minDistance and minVolume in maxDistance
    }

    public void UpdateVolume()
    {
        if (!photonView.IsMine) return;

        if(otherPlayer !=  null)
        {
            var distance = Vector3.Distance(transform.position, otherPlayer.transform.position);
            var newVolume = VolumeValue(distance);
            otherPlayerSource.volume = newVolume;
        }
    }

    private void UpdateOtherPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if(!player.GetPhotonView().AmOwner)
            {
                otherPlayer = player;
            }
        }
    }
}
