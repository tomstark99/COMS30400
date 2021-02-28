using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public GameObject playerUI;
    private GameObject uiRef;
    [SyncVar]
    public string gesture;

    private NetworkManagerMain room;
    private NetworkManagerMain Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerMain;
        }
    }

    public override void OnStartLocalPlayer()
    {
        uiRef = Instantiate(playerUI);
        gesture = PoseParser.GETGestureAsString();
        base.OnStartLocalPlayer();
    }

    public void SetPressE()
    {
        if (isLocalPlayer)
        {
            uiRef.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void UnsetPressE()
    {
        if (isLocalPlayer)
        {
            uiRef.transform.GetChild(1).gameObject.SetActive(false);
        }
        
    }

    [Command]
    public void getGesture(string temp)
    {
        gesture = temp;
    }

    [ClientCallback]
    void Update()
    {
        getGesture(PoseParser.GETGestureAsString());
    }

    //public override void OnStopClient()
    //{
    //    CmdRemovePlayer(this);
    //}

    //[Command]
    //private void CmdAddPlayer(Player player)
    //{

    //    Room.GamePlayers.Add(player);
    //}

    //[Command]
    //private void CmdRemovePlayer(Player player)
    //{
    //    Room.GamePlayers.Remove(this);
    //}
}