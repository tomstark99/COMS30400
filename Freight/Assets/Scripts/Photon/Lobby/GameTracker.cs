using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameTracker : MonoBehaviourPun
{
    private int playerCountFirst;
    private int playerCountSecond;

    private static GameTracker gameTrackerInstance;

    public int PlayerCountFirst
    {
        get { return playerCountFirst; }
    }

    public int PlayerCountSecond
    {
        get { return playerCountSecond; }
    }
   

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        
        // checks if it already exists, if it does we destroy it
        if (gameTrackerInstance == null)
        {
            gameTrackerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerCountFirst = 0;
        playerCountSecond = 0;
    }

    [PunRPC]
    void PlayerLoadedFirstLevelRPC()
    {
        Debug.Log("player loaded first scene");
        playerCountFirst += 1;
    }

    public void PlayerLoadedFirstLevel()
    {
        photonView.RPC(nameof(PlayerLoadedFirstLevelRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    void PlayerLoadedSecondLevelRPC()
    {
        Debug.Log("player loaded second scene");
        playerCountSecond += 1;
    }

    public void PlayerLoadedSecondLevel()
    {
        photonView.RPC(nameof(PlayerLoadedSecondLevelRPC), RpcTarget.AllBuffered);
    }
}
