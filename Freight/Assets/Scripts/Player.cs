using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    private NetworkManagerMain room;
    private NetworkManagerMain Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerMain;
        }
    }

    //public override void OnStartClient()
    //{
    //    DontDestroyOnLoad(gameObject);
        
    //    Room.GamePlayers.Add(this);
    //    CmdAddPlayer(this);
    //}

    //public override void OnStopClient()
    //{
    //    Room.GamePlayers.Remove(this);
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