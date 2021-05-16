using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameSettings : MonoBehaviour
{

    public void SetGameSettings()
    {
       // Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties);
        string guardDifficultyVal;
        if (PhotonNetwork.CurrentRoom.CustomProperties["sliderValueDiff"] != null)
            guardDifficultyVal = PhotonNetwork.CurrentRoom.CustomProperties["sliderValueDiff"].ToString();
        else
            guardDifficultyVal = "Tutorial";

        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();

        // easy
        if (guardDifficultyVal == "Easy")
        {
            prop.Add("GuardSightRange", 10);
            prop.Add("GuardAngle", 50);
            prop.Add("SpeedChasing", 4);
            prop.Add("SpeedPatrolling", 2);
            prop.Add("SpotlightsActive", true);
            prop.Add("SpotlightsRotating", false);
            prop.Add("VoiceRangeMultiplier", 0.1f);
            prop.Add("BackPackIcon", true);
        }
        // medium
        else if (guardDifficultyVal == "Medium")
        {
            prop.Add("GuardSightRange", 20);
            prop.Add("GuardAngle", 65);
            prop.Add("SpeedChasing", 6);
            prop.Add("SpeedPatrolling", 3);
            prop.Add("SpotlightsActive", true);
            prop.Add("SpotlightsRotating", true);
            prop.Add("VoiceRangeMultiplier", 0.5f);
            prop.Add("BackPackIcon", true);
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
            prop.Add("VoiceRangeMultiplier", 1f);
            prop.Add("BackPackIcon", false);
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
            prop.Add("VoiceRangeMultiplier", 2f);
            prop.Add("BackPackIcon", false);
        }
        // tutorial
        else if (guardDifficultyVal == "Tutorial")
        {
            prop.Add("GuardSightRange", 0);
            prop.Add("GuardAngle", 80);
            prop.Add("SpeedChasing", 7);
            prop.Add("SpeedPatrolling", 4);
            prop.Add("SpotlightsActive", false);
            prop.Add("SpotlightsRotating", false);
            prop.Add("VoiceRangeMultiplier", 0f);
            prop.Add("BackPackIcon", false);
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties["sliderValue"] != null)
        {
            float timeToLeave = (float)PhotonNetwork.CurrentRoom.CustomProperties["sliderValue"];
            prop.Add("TimeToLeave", timeToLeave);
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

    }
}
