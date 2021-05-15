using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBreakFenceSound1 : MonoBehaviour
{
    [SerializeField]
    private GameObject fenceBreakSound;

    private AudioSource fenceBreak;

    // Start is called before the first frame update
    void Start()
    {
        fenceBreak = fenceBreakSound.GetComponent<AudioSource>();
        fenceBreak.Play();
    }

}
