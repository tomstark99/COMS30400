using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FrostweepGames.Plugins.Native;
using FrostweepGames.WebGLPUNVoice;
using System.Linq;
using System;
using VoiceChatClass;

public class ProximityVoice : MonoBehaviourPun
{
    private GameObject otherPlayer = null;

    public float minDistance = 1f;
    public float maxDistance = 50f;
    private readonly float maxVolume = 1f;
    private readonly float minVolume = 0.01f;

    private float a;
    private float b;

    private readonly float updateFrequency = 0.1f;

    void OnDestroy()
    {
        if (!photonView.IsMine) return;

        VoiceChat.Instance.OnStatusUpdate -= OnStatusChanged;
    }

    void Start()
    {
        if (!photonView.IsMine) return;

        maxDistance = 50f;

        VoiceChat.Instance.OnStatusUpdate += OnStatusChanged;
        OnStatusChanged(VoiceChat.Instance.Status);

        //b = (minVolume - maxVolume) / (1 / Mathf.Sqrt(maxDistance) - 1 / Mathf.Sqrt(minDistance));
        //a = maxVolume - b / Mathf.Sqrt(minDistance);
        b = (minVolume - maxVolume) / (minDistance - maxDistance);
        a = maxVolume + b * minDistance;

        InvokeRepeating(nameof(UpdateVolume), 0, updateFrequency); // call it 1 / updateFrequency times per second
    }

    private float VolumeValue(float distance)
    {
        if (distance < minDistance) return 1f; //Max volume
        if (distance > maxDistance) return 0f; //Min volume
        //return a + b / Mathf.Sqrt(distance); // maxVolume in minDistance and minVolume in maxDistance
        return a - b * distance;
    }

    public void UpdateVolume()
    {
        if (!photonView.IsMine) return;

        if(otherPlayer !=  null)
        {
            var distance = Vector3.Distance(transform.position, otherPlayer.transform.position);
            var newVolume = VolumeValue(distance);
            if (0f <= newVolume && newVolume <= 1f) {
                VoiceChat.Instance.SetVolumeOfCall(newVolume);
            }
        }
    }

    private void OnStatusChanged(string status)
    {
        if (status == "connected")
        {
            UpdateOtherPLayer();
        }
    }

    private void UpdateOtherPLayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if(players.Length > 1)
        {

            foreach (GameObject player in players)
            {
                if (!player.GetPhotonView().IsMine)
                {
                    otherPlayer = player;
                }
            }
        }
        else
        {
            Invoke(nameof(UpdateOtherPLayer), 1);
        }
    }
}
