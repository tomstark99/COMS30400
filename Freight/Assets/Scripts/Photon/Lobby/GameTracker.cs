using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameTracker : MonoBehaviourPun
{
    private int playerCountFirst;
    private int playerCountSecond;

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
}
