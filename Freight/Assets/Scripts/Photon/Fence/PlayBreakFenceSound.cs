using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
    This class is placed on the broken fence prefab and its purpose is to play audio sources for when the fence is broken 
*/

public class PlayBreakFenceSound : MonoBehaviour
{
    [SerializeField]
    private GameObject fenceBreakSound;
    [SerializeField]
    private GameObject findTheBagsSound;

    private AudioSource fenceBreak;
    private AudioSource findTheBags;

    // Start is called before the first frame update
    void Start()
    {
        // gets the fence breaking sound effect and plays it 
        fenceBreak = fenceBreakSound.GetComponent<AudioSource>();
        fenceBreak.Play();
        
        // checks if we are in the tutorial, if so we return the function early as we do not want to play the next audio clip
        if (PhotonNetwork.CurrentRoom.CustomProperties["curScn"].ToString() == "Scenes/TutorialScene")
            return;

        // gets the voiceover that tells the user to find the bags in the station and plays it
        findTheBags = findTheBagsSound.GetComponent<AudioSource>();
        findTheBags.Play();
    }

}
