using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;

public class SceneObject : NetworkBehaviour
{
    private List<Player> players;
    private NetworkManagerMain room;
    private bool entered;
    public GameObject text;

    //sync the item on the server to all clients
    [SyncVar(hook = nameof(onChangeItem))]
    public EquippedItem equippedItem;

    private NetworkManagerMain Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerMain;
        }
    }


    //rock prefab
    public GameObject rockPrefab;

    [ServerCallback]
    void Start()
    {
        players = Room.GamePlayers;
        entered = false;
    }

    void onChangeItem(EquippedItem oldEquippedItem, EquippedItem newEquippedItem)
    {

        StartCoroutine(ChangeEquipment(newEquippedItem));

    }

    //change item
    IEnumerator ChangeEquipment(EquippedItem newEquippedItem)
    {

        //destroy anything the hand is holding
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return null;
        }

        // Use the new value, not the SyncVar property value
        SetEquippedItem(newEquippedItem);
    }
   
    public void SetEquippedItem(EquippedItem newEquippedItem)
    {
        //attach item to scene
        switch (newEquippedItem)
        {
            case EquippedItem.rock:

                Instantiate(rockPrefab, transform);
                break;

        }
    }

    [TargetRpc]
    void SetPressEToActive(NetworkConnection conn, Player player)
    {
        //player.SetPressE();
        text.SetActive(true);
    }

    [TargetRpc]
    void SetPressEToNotActive(NetworkConnection conn, Player player)
    {
        //player.UnsetPressE();
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
                SetPressEToActive(player.connectionToClient, player);
                //player.displaying = true;
                //entered = true;
                //text.SetActive(true);
            }
            else if (tempDist > 2.5f)
            {
                Debug.Log("falo");
                SetPressEToNotActive(player.connectionToClient, player);
                //player.displaying = false;
                //entered = false;
                //text.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector3.Distance(NetworkClient.connection.identity.transform.position, transform.position);
            if(dist <= 2.5f)
            {
                Debug.Log(gameObject);
                equippedItem = EquippedItem.rock;
                Debug.Log("onKeyDown");
                Debug.Log(NetworkClient.connection.identity);
                NetworkClient.connection.identity.GetComponent<ItemPickUp>().CmdPickupItem(gameObject);
            }
        }
    }
    void OnMouseDown()
    {
        
        Debug.Log("onMouseDown");
    }
}