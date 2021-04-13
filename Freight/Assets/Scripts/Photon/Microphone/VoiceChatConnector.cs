﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VoiceChatClass;
using FrostweepGames.Plugins.Native;
using System;

public class VoiceChatConnector : MonoBehaviourPun
{
    public AudioListener audioListener;
    public AudioSource foreignAudioSource;

    VoiceChat voiceChat;
    string _id = "";
    string _foreignID = "";
    private string _microphoneDevice;

    private AudioClip audioClip;

    public AudioClip SelfAudioClip() { return audioClip; }

    private readonly int lengthSec = 1;
    private readonly int frequency = 44100;
    private readonly int samplesPerCall = 44100;

    private void OnDestroy()
    {
        CustomMicrophone.End(_microphoneDevice);
        voiceChat.EndCall();
    }

    void Start()
    {
        if (!photonView.IsMine) return;

        audioListener.enabled = true;
        foreignAudioSource.enabled = true;

        if (!CustomMicrophone.HasConnectedMicrophoneDevices())
        {
            CustomMicrophone.RefreshMicrophoneDevices();
        }
        Debug.Log(CustomMicrophone.devices.Length + " microphone devices found");

        if (!CustomMicrophone.HasConnectedMicrophoneDevices())
        {
            Debug.Log("no microphone device connected");
        }

        _microphoneDevice = CustomMicrophone.devices[0];
        audioClip = CustomMicrophone.Start(_microphoneDevice, true, lengthSec, frequency);

        //get instance 
        voiceChat = VoiceChat.Instance;
        
        //subscribe to events
        voiceChat.OnStatusUpdate += OnStatusupdate;
        voiceChat.OnIDUpdate += OnIDUpdate;

        //initialize peer object for this client
        voiceChat.InitializePeer();
    }

    void OnStatusupdate(string status)
    {
        if(status == "connected")
        {
            Debug.Log("connected to peer");
        }
        else if(status == "disconnected")
        {
            Debug.Log("disconnected from the peer");
            if (PhotonNetwork.IsMasterClient)
                voiceChat.Connect(_foreignID);
        }
        else if (status == "destroyed")
        {
            Debug.Log("peer crashed");
        }
    }

    void GetForeignPeerID(string ID)
    {
        _foreignID = ID;

        voiceChat.Call(_foreignID);
    }

    [PunRPC]
    void GetForeignPeerIDRPC(string ID)
    {
        // this will be called on Master client on the player object of client 
        // so we neeed to forward it to the player object of master
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetPhotonView().IsMine)
            {
                VoiceChatConnector voice = player.GetComponent<VoiceChatConnector>();
                voice.GetForeignPeerID(ID);
            }
        }
    }

    void OnIDUpdate(string ID)
    {
        _id = ID;
        Debug.Log("got id in unity: " + ID);

        // send id to master
        if(!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(GetForeignPeerIDRPC), RpcTarget.MasterClient, ID);
        }
    }
}