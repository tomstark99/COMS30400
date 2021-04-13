using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FrostweepGames.WebGLPUNVoice;
using FrostweepGames.Plugins.Native;
using System.Linq;

public class SoundRipples : MonoBehaviourPun
{
    public VoiceChatConnector voice;
    public ParticleSystem particles;

    public float decibelsValue = 0f;

    private int _lastPosition = 0;
    private string _microphoneDevice;
    private readonly float _updateFrequency = 0.25f;

    //checks if the decibels value was positive in the last second
    //if not, using a refPower of 0.001f, means that no sound was made
    //see ComputeDB line 6
    public bool positiveInLastSecond = false;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) return;

        _microphoneDevice = CustomMicrophone.devices[0];

        InvokeRepeating(nameof(UpdateRipples), 0, _updateFrequency);
    }

    private void UpdateRipples()
    {
        if (!photonView.IsMine) return;

        int currentPosition = CustomMicrophone.GetPosition(_microphoneDevice);

        //cheks how many samples were recorder since last time we calculated
        //if it went around the buffer (so past the limit and back to 0) we just 
        //do the rest of the array and next time start from 0
        if(currentPosition != _lastPosition)
        {
            int length = 1 * 44100;
            float[] data = new float[length];

            // only works if audioClip is the one where this microphone records
            CustomMicrophone.GetRawData(ref data, voice.SelfAudioClip());

            if (currentPosition > _lastPosition)
            {
                int len = currentPosition - _lastPosition;
                decibelsValue = ComputeDB(data, _lastPosition, ref len);
                _lastPosition = currentPosition;
            } 
            else
            { 
                int len = data.Length - _lastPosition;
                decibelsValue = ComputeDB(data, _lastPosition, ref len);
                _lastPosition = 0;
            }

            //udpate sound ripples animation on all clients
            photonView.RPC(nameof(UpdateSoundRiples), RpcTarget.All, decibelsValue);

            if(count < 1 / _updateFrequency)
            {
                count++;
                positiveInLastSecond |= decibelsValue > 0;
            }
            else
            {
                count = 1;
                positiveInLastSecond = decibelsValue > 0;
            }
        }

    }

    [PunRPC]
    private void UpdateSoundRiples(float decibels)
    {
        if (decibels <= 0)
        {
            ParticleSystem.MainModule main = particles.main;
            main.startSize = 1;
        }
        else
        {
            ParticleSystem.MainModule main = particles.main;
            main.startSize = decibels;
        }
    }

    private float ComputeRMS(float[] buffer, int offset, ref int length)
    {
        // sum of squares
        float sos = 0f;
        float val;

        if (offset + length > buffer.Length)
        {
            length = buffer.Length - offset;
        }

        for (int i = 0; i < length; i++)
        {
            val = buffer[offset];
            sos += val * val;
            offset++;
        }

        // return sqrt of average
        return Mathf.Sqrt(sos / length);
    }

    private float ComputeDB(float[] buffer, int offset, ref int length)
    {
        float rms;

        rms = ComputeRMS(buffer, offset, ref length);

        //0.001f works really well, if no sound is made, result will be below 0
        //with wispers, the result will be between 10 - 20, and normal talking will be above 20 
        //friendly reminder that decibels values are relative to a chosen "basic" value
        float refPower = 0.001f;
        return 10 * Mathf.Log10(rms / refPower);
    }
}
