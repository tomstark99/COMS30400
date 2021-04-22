using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Achievements : MonoBehaviourPun
{
    [SerializeField]
    private GameObject achievements;
    [SerializeField]
    private GameObject achievementsTab;

    private GameObject babySteps;
    private GameObject letTheHuntBegin;
    private GameObject useNature;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            var tempColor = achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            tempColor.a = 1f;

            if (PlayerPrefs.HasKey("BabySteps"))
            {
                achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("LetTheHuntBegin"))
            {
                achievementsTab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("UseNature"))
            {
                achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
            }
            babySteps = achievements.transform.GetChild(0).gameObject;
            letTheHuntBegin = achievements.transform.GetChild(1).gameObject;
            useNature = achievements.transform.GetChild(3).gameObject;
        }
    }

    public void BabyStepsCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("BabySteps"))
            {
                PlayerPrefs.SetInt("BabySteps", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
                StartCoroutine(BabyStepsSequence());
            }

        }
    }

    IEnumerator BabyStepsSequence()
    {
        babySteps.SetActive(true);
        yield return new WaitForSeconds(3f);
        babySteps.SetActive(false);
    }

    public void LetTheHuntBeginCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("LetTheHuntBegin"))
            {
                PlayerPrefs.SetInt("LetTheHuntBegin", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
                StartCoroutine(LetTheHuntBeginSequence());
            }

        }
    }

    IEnumerator LetTheHuntBeginSequence()
    {
        letTheHuntBegin.SetActive(true);
        yield return new WaitForSeconds(3f);
        letTheHuntBegin.SetActive(false);
    }

    public void UseNatureCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("UseNature"))
            {
                PlayerPrefs.SetInt("UseNature", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
                StartCoroutine(UseNatureSequence());
            }
        }
    }

    IEnumerator UseNatureSequence()
    {
        useNature.SetActive(true);
        yield return new WaitForSeconds(3f);
        useNature.SetActive(false);
    }

}
