using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 


//script attached to the red alarm spotlights
public class RotateLightAlarm : MonoBehaviourPun
{
    [SerializeField]
    private GameObject spotlight;
    private bool positiveRotation;

    [SerializeField]
    private float rotationMultiplier;
    public float rotationUpperLimit;
    public float rotationLowerLimit;

    [SerializeField]
    private GameObject pointLight;
    [SerializeField]
    private GameObject spotLight;

    [SerializeField]
    private GameObject pointLight2;
    [SerializeField]
    private GameObject spotLight2;

    
    public bool hasSpinned;
    public bool isSpinning;
    
    void Start()
    {
        hasSpinned = false;
        positiveRotation = true;
        isSpinning = false;

        //find spinning lights 
        //if playerInLight is called on light, call SetToSpinning
        GameObject[] lights = GameObject.FindGameObjectsWithTag("SpinningLight");

        foreach (var light in lights)
        {
            light.GetComponent<RotateLight>().PlayerInLight += SetToSpinning;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (isSpinning)
        {
            Rotate();
        }
    }

    //rotate SpotLIghts
    void Rotate()
    {
        if (positiveRotation == true)
        {
            spotlight.transform.Rotate(Vector3.up * rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y + 40 > rotationUpperLimit)
            {

                positiveRotation = false;
            }
        }
        else
        {
            spotlight.transform.Rotate(Vector3.up * -rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y - 20 < rotationLowerLimit)
            {
                positiveRotation = true;
            }
        }
    }

    //Set alarm light to be spinning on both clients
    void SetToSpinning()
    {
        if(!transform.GetComponent<AudioSource>().isPlaying)
            photonView.RPC(nameof(SetToSpinningRPC), RpcTarget.All);
    }

    [PunRPC]
    void SetToSpinningRPC() 
    {
         if(hasSpinned == false)
            transform.GetComponent<AudioSource>().Play();

        hasSpinned = true;
        isSpinning = true;
        pointLight.SetActive(true);
        spotLight.SetActive(true);
        pointLight2.SetActive(true);
        spotLight2.SetActive(true);
        StartCoroutine(StopLight());
    }
    IEnumerator StopLight() {
        
        //spot the lights sound clip and light after 20 seconds
        yield return new WaitForSeconds(20);
        transform.GetComponent<AudioSource>().Stop();
        isSpinning = false;
        pointLight.SetActive(false);
        spotLight.SetActive(false);
        pointLight2.SetActive(false);
        spotLight2.SetActive(false);
        
    }
}
