using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
        fenceBreak = fenceBreakSound.GetComponent<AudioSource>();
        fenceBreak.Play();
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties);
        if (PhotonNetwork.CurrentRoom.CustomProperties["curScn"].ToString() == "Scenes/TutorialScene")
            return;

        findTheBags = findTheBagsSound.GetComponent<AudioSource>();
        findTheBags.Play();
    }

}
