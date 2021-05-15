using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaratorVoiceAudio : MonoBehaviour
{
    float naratorAudioVolume;
    // Start is called before the first frame update
    void Start() 
    {
        if(PlayerPrefs.HasKey("naratorAudio")) {
            naratorAudioVolume = PlayerPrefs.GetFloat("narratorAudio");
        } else 
        naratorAudioVolume = 1;

        foreach (Transform child in transform)
        {
            child.GetComponent<AudioSource>().volume = naratorAudioVolume;
        }
    }
}
