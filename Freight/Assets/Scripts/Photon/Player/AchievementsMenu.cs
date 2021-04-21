using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class AchievementsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var tempColor = transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        tempColor.a = 1f;
        // checking for completed achievements
        if (PlayerPrefs.HasKey("BabySteps"))
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        }

        if (PlayerPrefs.HasKey("UseNature"))
        {
            transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
        }
    }

    public void ResetAchievements()
    {
        var tempColor = transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        tempColor.a = 0.11764705882f;
        PlayerPrefs.DeleteKey("BabySteps");
        PlayerPrefs.DeleteKey("UseNature");
        transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
