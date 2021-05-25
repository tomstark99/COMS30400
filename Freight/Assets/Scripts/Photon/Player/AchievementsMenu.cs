using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//this script manages the achievements icons color on the main menu
public class AchievementsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var tempColor = transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        tempColor.a = 1f;
        // checking for completed achievements
        if (PlayerPrefs.HasKey("BabySteps1"))
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("LetTheHuntBegin1"))
        {
            transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("PeaceTreaty1"))
        {
            transform.GetChild(0).GetChild(2).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("UseNature1"))
        {
            transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("Roadman1"))
        {
            transform.GetChild(0).GetChild(4).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("TheCompletePicture1"))
        {
            transform.GetChild(0).GetChild(5).GetComponent<Image>().color = tempColor;
        }

        if (PlayerPrefs.HasKey("LikeANinja1"))
        {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("Hackerman1"))
        {
            transform.GetChild(1).GetChild(1).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("LearnTheHardWay1"))
        {
            transform.GetChild(1).GetChild(2).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("ChooChoo1"))
        {
            transform.GetChild(1).GetChild(3).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("OnTheRun1"))
        {
            transform.GetChild(1).GetChild(4).GetComponent<Image>().color = tempColor;
        }
        if (PlayerPrefs.HasKey("Freight1"))
        {
            transform.GetChild(1).GetChild(5).GetComponent<Image>().color = tempColor;
        }
    }

    // on button click resets all achievements to unachieved
    public void ResetAchievements()
    {
        var tempColor = transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        // sets the alpha of the colour to be low to make the achievement appear to be greyed out
        tempColor.a = 0.11764705882f;

        // delete all the achievements
        PlayerPrefs.DeleteKey("BabySteps1");
        PlayerPrefs.DeleteKey("LetTheHuntBegin1");
        PlayerPrefs.DeleteKey("PeaceTreaty1");
        PlayerPrefs.DeleteKey("UseNature1");
        PlayerPrefs.DeleteKey("Roadman1");
        PlayerPrefs.DeleteKey("TheCompletePicture1");

        PlayerPrefs.DeleteKey("LikeANinja1");
        PlayerPrefs.DeleteKey("Hackerman1");
        PlayerPrefs.DeleteKey("LearnTheHardWay1");
        PlayerPrefs.DeleteKey("ChooChoo1");
        PlayerPrefs.DeleteKey("OnTheRun1");
        PlayerPrefs.DeleteKey("Freight1");

        // loops through all achievements and sets them to have lower alpha
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                transform.GetChild(i).GetChild(j).GetComponent<Image>().color = tempColor;
            }
        }
    }


}
