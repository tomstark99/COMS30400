using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAudioClips : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI subtitle;
    [SerializeField]
    private GameObject intro;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1f);
        subtitle.text = "Bossman: Mission's simple, get in, get the bags, get out, any mean's necessary";
        intro.SetActive(true);
        yield return new WaitForSeconds(5f);
        subtitle.text = "";
    }
}
