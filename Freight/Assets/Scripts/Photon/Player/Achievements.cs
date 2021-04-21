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

    // Start is called before the first frame update
    void Start()
    {
        babySteps = achievements.transform.GetChild(0).gameObject;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
