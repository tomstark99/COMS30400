using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameSettings : MonoBehaviour
{

    void SetGameSettings()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties);
        string guardDifficultyVal = PhotonNetwork.CurrentRoom.CustomProperties["sliderValueDiff"].ToString();
        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();

        // easy
        if (guardDifficultyVal == "Easy")
        {
            prop.Add("GuardSightRange", 10);
            prop.Add("GuardAngle", 50);
            prop.Add("SpeedChasing", 4);
            prop.Add("SpeedPatrolling", 2);
            prop.Add("SpotlightsActive", false);
            prop.Add("SpotlightsRotating", false);
        }
        // medium
        else if (guardDifficultyVal == "Medium")
        {
            prop.Add("GuardSightRange", 20);
            prop.Add("GuardAngle", 65);
            prop.Add("SpeedChasing", 6);
            prop.Add("SpeedPatrolling", 3);
            prop.Add("SpotlightsActive", true);
            prop.Add("SpotlightsRotating", false);
        }
        // hard
        else if (guardDifficultyVal == "Hard")
        {
            prop.Add("GuardSightRange", 30);
            prop.Add("GuardAngle", 80);
            prop.Add("SpeedChasing", 7);
            prop.Add("SpeedPatrolling", 4);
            prop.Add("SpotlightsActive", true);
            prop.Add("SpotlightsRotating", true);
        }
        // impossible
        else if (guardDifficultyVal == "Impossible")
        {
            prop.Add("GuardSightRange", 40);
            prop.Add("GuardAngle", 100);
            prop.Add("SpeedChasing", 10);
            prop.Add("SpeedPatrolling", 6);
            prop.Add("SpotlightsActive", true);
            prop.Add("SpotlightsRotating", true);
        }

        float timeToLeave = (float) PhotonNetwork.CurrentRoom.CustomProperties["sliderValue"];
        prop.Add("TimeToLeave", timeToLeave);
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

    }
}
