using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BreakFence : NetworkBehaviour
{
    public GameObject brokenFencePrefab;
    private List<Player> players;
    private NetworkManagerMain room;
    public GameObject text;

    private NetworkManagerMain Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerMain;
        }
    }

    [ServerCallback]
    void Start()
    {
        players = Room.GamePlayers;
    }

    [TargetRpc]
    void SetPressQToActive(NetworkConnection conn)
    {
        text.SetActive(true);
    }

    [TargetRpc]
    void SetPressQToNotActive(NetworkConnection conn)
    {
        text.SetActive(false);
    }

    void Update()
    {
        players = Room.GamePlayers;

        foreach (Player player in players)
        {
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            if (tempDist <= 2.5f)
            {
                SetPressQToActive(player.connectionToClient);
            }
            else if (tempDist > 2.5f)
            {
                SetPressQToNotActive(player.connectionToClient);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            float dist = Vector3.Distance(NetworkClient.connection.identity.transform.position, transform.position);
            if (dist <= 2.5f)
            {
                Vector3 spawnPosition = transform.position;
                Destroy(transform.gameObject);
                GameObject brokenFence = Instantiate(brokenFencePrefab, spawnPosition, Quaternion.Euler(0f, 90f, 0f));
                NetworkServer.Spawn(brokenFence);
            }
        }
    }

}

