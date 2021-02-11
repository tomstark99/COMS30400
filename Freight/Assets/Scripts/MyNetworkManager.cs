using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public List<Transform> players = new List<Transform>();

    List<Transform> GetPlayers()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
        List<Transform> temp_players = new List<Transform>();
        foreach (GameObject t in temp)
        {
            temp_players.Add(t.transform);
        }
        return temp_players;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        players = GetPlayers();
    }
}
