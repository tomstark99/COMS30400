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
    private GameObject laptop;
    [SerializeField]
    private GameObject leave;

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
}
