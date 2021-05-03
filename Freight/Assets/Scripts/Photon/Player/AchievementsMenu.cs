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
        if (PlayerPrefs.HasKey("LetTheHuntBegin"))
        {
            transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("PeaceTreaty"))
        {
            transform.GetChild(0).GetChild(2).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("UseNature"))
        {
            transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("Roadman"))
        {
            transform.GetChild(0).GetChild(4).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("TheCompletePicture"))
        {
            transform.GetChild(0).GetChild(5).GetComponent<Image>().color = tempColor;
        }

        if (PlayerPrefs.HasKey("LikeANinja"))
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("Hackerman"))
        {
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("LearnTheHardWay"))
        {
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("ChooChoo"))
        {
            transform.GetChild(1).GetChild(3).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("OnTheRun"))
        {
            transform.GetChild(1).GetChild(4).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("Freight"))
        {
            transform.GetChild(1).GetChild(5).GetComponent<Image>().color = tempColor;
        }
    }

    public void ResetAchievements()
    {
        var tempColor = transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        tempColor.a = 0.11764705882f;

        PlayerPrefs.DeleteKey("BabySteps");
        PlayerPrefs.DeleteKey("LetTheHuntBegin");
        PlayerPrefs.DeleteKey("PeaceTreaty");
        PlayerPrefs.DeleteKey("UseNature");
        PlayerPrefs.DeleteKey("Roadman");
        PlayerPrefs.DeleteKey("TheCompletePicture");

        PlayerPrefs.DeleteKey("LikeANinja");
        PlayerPrefs.DeleteKey("Hackerman");
        PlayerPrefs.DeleteKey("LearnTheHardWay");
        PlayerPrefs.DeleteKey("ChooChoo");
        PlayerPrefs.DeleteKey("OnTheRun");
        PlayerPrefs.DeleteKey("Freight");

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                transform.GetChild(i).GetChild(j).GetComponent<Image>().color = tempColor;
            }
        }

        //transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        //transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
        //transform.GetChild(0).GetChild(2).GetComponent<Image>().color = tempColor;
        //transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
        //transform.GetChild(0).GetChild(4).GetComponent<Image>().color = tempColor;
        //transform.GetChild(0).GetChild(5).GetComponent<Image>().color = tempColor;

        //transform.GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
        //transform.GetChild(1).GetChild(1).GetComponent<Image>().color = tempColor;
        //transform.GetChild(1).GetChild(2).GetComponent<Image>().color = tempColor;
        //transform.GetChild(1).GetChild(3).GetComponent<Image>().color = tempColor;
        //transform.GetChild(1).GetChild(4).GetComponent<Image>().color = tempColor;
        //transform.GetChild(1).GetChild(5).GetComponent<Image>().color = tempColor;
    }


}
