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
        if (PlayerPrefs.HasKey("BabySteps"))
        {
            var tempColor = transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            tempColor.a = 1f;
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
