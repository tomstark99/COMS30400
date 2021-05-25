using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/*
    This class is responsible for playing the voiceovers during the level and displaying the subtitles 
*/
public class PlayerAudioClips : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI subtitle;
    [SerializeField]
    private GameObject intro;
    [SerializeField]
    private GameObject laptop;
    [SerializeField]
    private GameObject leave;
    [SerializeField]
    private GameObject spotlightDetected;

    // Start is called before the first frame update
    void Start()
    {
        // if there is a broken fence in the level, we subscribe to the fence breaking event
        if (GameObject.FindGameObjectWithTag("BrokenFence") != null)
            GameObject.FindGameObjectWithTag("BrokenFence").GetComponent<Breakable>().FenceBroke += FindTheBags;

        // start coroutine to play initial voiceover
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1f);
        subtitle.text = "Bossman: Mission's simple, get in, get the bags, get out, any means necessary";
        intro.SetActive(true);
        yield return new WaitForSeconds(5f);
        subtitle.text = "";
    }

    public void LaptopFound()
    {
        StartCoroutine(LaptopSequence());
    }

    IEnumerator LaptopSequence()
    {
        subtitle.text = "Bossman: Alright this laptop controls the spotlights, get on the laptop and turn them off";
        laptop.SetActive(true);
        yield return new WaitForSeconds(4f);
        subtitle.text = "";
    }

    public void BagsCollected()
    {
        StartCoroutine(BagSequence());
    }

    IEnumerator BagSequence()
    {
        subtitle.text = "Bossman: You got everything we need, find the train it should be leaving soon!";
        leave.SetActive(true);
        yield return new WaitForSeconds(3f);
        subtitle.text = "";
    }

    public void FindTheBags()
    {
        StartCoroutine(FindTheBagsSequence());
    }

    IEnumerator FindTheBagsSequence()
    {
        subtitle.text = "Bossman: Good job on making your way in, the bags should be in the buildings!";
        yield return new WaitForSeconds(4f);
        subtitle.text = "";
    }

    public void SpottedByLights()
    {
        StartCoroutine(SpottedByLightsSequence());
    }

    IEnumerator SpottedByLightsSequence()
    {
        subtitle.text = "Bossman: You've been detected by the spotlights, the guards are gonna be after you";
        spotlightDetected.SetActive(true);
        yield return new WaitForSeconds(3f);
        subtitle.text = "";
    }
}
