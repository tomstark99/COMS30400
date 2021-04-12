using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    private int guardSightRange;
    private int guardAngle;
    private int speedChasing;
    private int speedPatrolling;
    private int timeToLeave;
    private bool spotlightsRotating;
    private bool spotlightsActive;

    public int GuardSightRange
    {
        get { return guardSightRange; }
    }
    public int GuardAngle
    {
        get { return guardAngle; }
    }
    public int SpeedChasing
    {
        get { return speedChasing; }
    }
    public int SpeedPatrolling
    {
        get { return speedPatrolling; }
    }
    public int TimeToLeave
    {
        get { return timeToLeave; }
    }
    public bool SpotlightsRotating
    {
        get { return spotlightsRotating; }
    }
    public bool SpotlightsActive
    {
        get { return spotlightsActive; }
    }


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetGameSettings()
    {
        GameObject guardDifficulty = GameObject.Find("Canvases/OverlayCanvases/CurrentRoomCanvas/GameSettingsObject/GuardDifficulty/Slider");
        int guardDifficultyVal = (int)guardDifficulty.GetComponent<Slider>().value;

        // easy
        if (guardDifficultyVal == 0)
        {
            guardSightRange = 10;
            guardAngle = 50;
            speedChasing = 4;
            speedPatrolling = 2;
            spotlightsActive = false;
            spotlightsRotating = false;
        } 
        // medium
        else if (guardDifficultyVal == 1)
        {
            guardSightRange = 20;
            guardAngle = 65;
            speedChasing = 6;
            speedPatrolling = 3;
            spotlightsActive = true;
            spotlightsRotating = false;
        }
        // hard
        else if (guardDifficultyVal == 2)
        {
            guardSightRange = 30;
            guardAngle = 80;
            speedChasing = 7;
            speedPatrolling = 4;
            spotlightsActive = true;
            spotlightsRotating = true;
        }
        // impossible
        else if (guardDifficultyVal == 3)
        {
            guardSightRange = 40;
            guardAngle = 100;
            speedChasing = 10;
            speedPatrolling = 6;
            spotlightsActive = true;
            spotlightsRotating = true;
        }

        GameObject timeToLeaveObject = GameObject.Find("Canvases/OverlayCanvases/CurrentRoomCanvas/GameSettingsObject/TimeToLeave/Slider");
        timeToLeave =  (int)timeToLeaveObject.GetComponent<Slider>().value;

        Debug.Log("Sight range: " + guardSightRange);
        Debug.Log("Angle: " + guardAngle);
        Debug.Log("Chasing Speed: " + speedChasing);
        Debug.Log("Patrolling Speed: " + speedPatrolling);
        Debug.Log("Spotlights active? " + spotlightsActive);
        Debug.Log("Spotlights rotating? " + spotlightsRotating);
        Debug.Log("Time to leave " + timeToLeave);

    }
}
