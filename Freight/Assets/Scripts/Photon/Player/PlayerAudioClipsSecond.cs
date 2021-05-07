using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerAudioClipsSecond : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI subtitle;
    [SerializeField]
    private GameObject intro;
    [SerializeField]
    private GameObject bagsDropped;
    [SerializeField]
    private GameObject gameFinished;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1f);
        subtitle.text = "Bossman: Find the drop points and drop off the bags, the locations should be on your display";
        intro.SetActive(true);
        yield return new WaitForSeconds(4f);
        subtitle.text = "";
    }

    public void BagsDroppedOff()
    {
        StartCoroutine(BagsDroppedOffSequence());
    }

    IEnumerator BagsDroppedOffSequence()
    {
        subtitle.text = "Bossman: Getaway vehicle is waiting on the road";
        bagsDropped.SetActive(true);
        yield return new WaitForSeconds(2f);
        subtitle.text = "";
    }

    public void GameFinished()
    {
        StartCoroutine(GameFinishedSequence());
    }

    IEnumerator GameFinishedSequence()
    {
        subtitle.text = "Bossman: Alright thats a job well done, get home safe";
        gameFinished.SetActive(true);
        yield return new WaitForSeconds(3f);
        subtitle.text = "";
    }

}
