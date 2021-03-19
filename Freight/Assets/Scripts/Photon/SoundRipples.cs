using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FrostweepGames.WebGLPUNVoice;
using FrostweepGames.Plugins.Native;
using System.Linq;

public class SoundRipples : MonoBehaviourPun
{
    public ParticleSystem particles;
    public Recorder recorder;

    public float decibelsValue = 0f;

    private int _lastPosition = 0;
    private string _microphoneDevice;
    private readonly float _updateFrequency = 0.25f;

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

        if(currentPosition != _lastPosition)
        {
            int length = Constants.RecordingTime * Constants.SampleRate;
            float[] data = new float[length];

            CustomMicrophone.GetRawData(ref data, recorder.AudioClip);

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
            //Debug.Log(decibelsValue);
            if (decibelsValue <= 0)
            {
                ParticleSystem.MainModule main = particles.main;
                main.startSize = 1;
            }
            else
            {
                ParticleSystem.MainModule main = particles.main;
                main.startSize = decibelsValue;
            }
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

        float refPower = 0.001f;
        return 10 * Mathf.Log10(rms / refPower);
    }
}
