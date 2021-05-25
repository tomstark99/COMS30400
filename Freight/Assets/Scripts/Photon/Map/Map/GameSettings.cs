using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameSettings : MonoBehaviour
{

    public void SetGameSettings()
    {
           
        string guardDifficultyVal;
        
        // if we have the difficulty of the game set to something in the custom properties, that means we are starting a normal game, if its set to nothing we are playing the tutorial
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

        // checks if there is a value for time to leave (if not then we are playing tutorial)
        if (PhotonNetwork.CurrentRoom.CustomProperties["sliderValue"] != null)
        {
            float timeToLeave = (float)PhotonNetwork.CurrentRoom.CustomProperties["sliderValue"];
            prop.Add("TimeToLeave", timeToLeave);
        }

        // apply game settings to room properties
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

    }
}
