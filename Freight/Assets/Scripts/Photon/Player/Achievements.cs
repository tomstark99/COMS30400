using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    [SerializeField]
    private GameObject achievements;
    [SerializeField]
    private GameObject achievementsTab;

    private GameObject babySteps;
    private GameObject useNature;

    // Start is called before the first frame update
    void Start()
    {
        var tempColor = achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        tempColor.a = 1f;

        if (PlayerPrefs.HasKey("BabySteps"))
        {
            achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("UseNature"))
        {
            achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
        }
        babySteps = achievements.transform.GetChild(0).gameObject;
        useNature = achievements.transform.GetChild(3).gameObject;
    }

    public void BabyStepsCompleted()
    {
        PlayerPrefs.SetInt("BabySteps", 1);
        PlayerPrefs.Save();
        var tempColor = achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        tempColor.a = 1f;
        achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        StartCoroutine(BabyStepsSequence());
    }

    IEnumerator BabyStepsSequence()
    {
        babySteps.SetActive(true);
        yield return new WaitForSeconds(3f);
        babySteps.SetActive(false);
    }

    public void UseNatureCompleted()
    {
        PlayerPrefs.SetInt("UseNature", 1);
        PlayerPrefs.Save();
        var tempColor = achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color;
        tempColor.a = 1f;
        achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
        StartCoroutine(UseNatureSequence());
    }

    IEnumerator UseNatureSequence()
    {
        useNature.SetActive(true);
        yield return new WaitForSeconds(3f);
        useNature.SetActive(false);
    }

}
