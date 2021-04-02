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
    private string _status = "disconnected";
    private string _microphoneDevice;

    private AudioClip foreignClip;
    private AudioClip audioClip;

    public AudioClip SelfAudioClip() { return audioClip; }
    public AudioClip ForeignAudioClip() { return foreignClip; }

    public Action OnStatusConnected;
    // Start is called before the first frame update

    private void OnDestroy()
    {
        CustomMicrophone.End(_microphoneDevice);
        voiceChat.Disconnect();
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
            //return;
        }

        _microphoneDevice = CustomMicrophone.devices[0];
        audioClip = CustomMicrophone.Start(_microphoneDevice, true, 1, 44100);

        //get instance 
        voiceChat = VoiceChat.Instance;

        //subscribe to events and initialize connection
        voiceChat.OnStatusUpdate += OnStatusupdate;
        voiceChat.OnIDUpdate += OnIDUpdate;

        foreignClip = voiceChat.GetClip(1, 44100);
        foreignAudioSource.clip = foreignClip;

        voiceChat.InitializePeer();
        //voiceChat.GetPeerId();
    }


    void OnStatusupdate(string status)
    {
        if(status == "connected")
        {
            Debug.Log("connected to peer");
            foreignAudioSource.Play();
            OnStatusConnected?.Invoke();
        }
        else if(status == "disconnected")
        {
            Debug.Log("disconnected from the peer");
            foreignAudioSource.Stop();
            voiceChat.Connect(_foreignID);
        }
        else if (status == "destroyed")
        {
            Debug.Log("peer crashed");
            // if this peer crashes, this should crash the other peer as well
            // so both will be reinitialized
            foreignAudioSource.Stop();
        }
    }

    void GetForeignPeerID(string ID)
    {
        _foreignID = ID;

        voiceChat.Connect(_foreignID);
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
