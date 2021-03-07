using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FrostweepGames.Plugins.Native;
using FrostweepGames.WebGLPUNVoice;
using System.Linq;

public class ProximityVoice : MonoBehaviourPun
{
    public Listener listener;
    public Recorder recorder;
    public AudioListener audioListener;

    private Dictionary<int, AudioSource> _sources;

    private readonly float minDistance = 1f;
    private readonly float maxDistance = 10f;
    private readonly float maxVolume = 1f;
    private readonly float minVolume = 0.01f;

    private float a;
    private float b;

    private readonly float updateFrequency = 0.1f;

    void Start()
    {
        if (!photonView.IsMine) return;

        listener.enabled = true;
        recorder.enabled = true;
        audioListener.enabled = true;

        listener.StartListen();
        recorder.StartRecord();

        _sources = new Dictionary<int, AudioSource>();
        listener.SpeakersUpdatedEvent += OnSpeakerUpdate;

        b = (minVolume - maxVolume) / (1 / Mathf.Sqrt(maxDistance) - 1 / Mathf.Sqrt(minDistance));
        a = maxVolume - b / Mathf.Sqrt(minDistance);

        InvokeRepeating(nameof(UpdateVolume), 0, updateFrequency); // call it 1 / updateFrequency times per second
    }

    private float VolumeValue(float distance)
    {
        if (distance < 1f) return 1f; //Max volume
        if (distance > 5f) return 0f; //Min volume
        return a + b / Mathf.Sqrt(distance); // maxVolume in minDistance and minVolume in maxDistance
    }


    public void UpdateVolume()
    {
        if (!photonView.IsMine) return;

        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            int Id = player.GetPhotonView().Owner.ActorNumber;
            //both for the player object of this owner and because order of calling functions it's a black box
            if (!_sources.ContainsKey(Id)) continue; 

            var distance = Vector3.Distance(transform.position, player.transform.position);
            _sources[Id].volume = VolumeValue(distance);
        }
    }

    private void OnSpeakerUpdate()
    {
        _sources = new Dictionary<int, AudioSource>();
        foreach(int id in listener.Speakers.Keys)
        {
            GameObject speaker = GameObject.Find(listener.Speakers[id].Name);
            AudioSource audiosource = speaker.GetComponent<AudioSource>();
            _sources.Add(id, audiosource);
        }
    }
}
