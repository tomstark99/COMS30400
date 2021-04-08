using System.Collections;
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

    private readonly int lengthSec = 1;
    private readonly int frequency = 44100;
    private readonly int samplesPerCall = 44100;
    private List<float> _buffer;
    private bool Playing;
    private float _delay;
    private int _samplePosition;

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
            //return;
        }

        _microphoneDevice = CustomMicrophone.devices[0];
        audioClip = CustomMicrophone.Start(_microphoneDevice, true, lengthSec, frequency);

        //get instance 
        voiceChat = VoiceChat.Instance;

        //_buffer = new List<float>();

        //subscribe to events and initialize connection
        voiceChat.OnStatusUpdate += OnStatusupdate;
        voiceChat.OnIDUpdate += OnIDUpdate;
        voiceChat.OnDataReceive += OnReceiveData;

        //foreignClip = AudioClip.Create("ForeignClip", frequency * lengthSec, 1, frequency, false);
        //foreignAudioSource.clip = foreignClip;
        //foreignAudioSource.clip = audioClip;

        //_samplePosition = 0;
        //Playing = false;

        voiceChat.InitializePeer();
    }

    private void FillBufffer()
    {
        var data = new float[frequency * lengthSec];    
        CustomMicrophone.GetRawData(ref data, audioClip);
        _buffer.AddRange(data);
    }

    private void AudioUpdate()
    {
        //foreignAudioSource.Stop();

        var bufferlength = _buffer.Count;
        Debug.Log(bufferlength);
        if (bufferlength > samplesPerCall)
        {
            foreignClip.SetData(_buffer.GetRange(0, samplesPerCall).ToArray(), 0);
            _samplePosition = (_samplePosition + samplesPerCall) % foreignClip.samples;
            _buffer.RemoveRange(0, samplesPerCall);

            if (!foreignAudioSource.isPlaying)  foreignAudioSource.Play();

            //Invoke(nameof(AudioUpdate), (float) samplesPerCall / (float) frequency);
        }
        else if(bufferlength > 0)
        {
            var data = new float[samplesPerCall];

            _buffer.GetRange(0, bufferlength).CopyTo(data);
            for (int i = bufferlength; i < samplesPerCall; i++)
                data[i] = 0;

            foreignClip.SetData(data, 0);
            _samplePosition = (_samplePosition + samplesPerCall) % foreignClip.samples;
            _buffer.RemoveRange(0, bufferlength);

            if (!foreignAudioSource.isPlaying)  foreignAudioSource.Play();

            //Invoke(nameof(AudioUpdate), (float) bufferlength / (float) frequency);
        } 
        else 
        {
            foreignAudioSource.Stop();
            _samplePosition = 0;

            //Invoke(nameof(AudioUpdate), 1f / 30f);
        }
    }

    //void Update()
    //{
    //    try
    //    {
    //        Debug.Log(_buffer.Count);
    //        var _audioClipReadyToUse = _buffer.Count >= frequency * lengthSec;

    //        if (Playing)
    //        {
    //            _delay -= Time.deltaTime;

    //            if (_delay <= 0)
    //            {
    //                foreignAudioSource.Stop();
    //                Playing = false;
    //            }
    //        }

    //        if (!Playing)
    //        {
    //            if (_audioClipReadyToUse)
    //            {
    //                List<float> chunk;

    //                if (_buffer.Count >= frequency)
    //                {
    //                    chunk = _buffer.GetRange(0, frequency);
    //                    _buffer.RemoveRange(0, frequency);
    //                }
    //                else
    //                {
    //                    chunk = _buffer.GetRange(0, _buffer.Count);
    //                    _buffer.Clear();
    //                    for (int i = chunk.Count; i < frequency; i++)
    //                    {
    //                        chunk.Add(0);
    //                    }
    //                }

    //                foreignClip.SetData(chunk.ToArray(), 0);
    //                foreignAudioSource.Play();

    //                _delay = (float)frequency / (float)chunk.Count;
    //                Playing = true;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogException(ex);
    //    }
    //}

    void OnReceiveData(float[] data)
    {
        //_buffer.AddRange(data);
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
