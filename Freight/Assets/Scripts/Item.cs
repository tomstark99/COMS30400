using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Item : NetworkBehaviour
{
    public GameObject weaponPrefab;
    private GameObject childrenWeapon;
    private GameObject thePlayers;
    void Start()  
    {
   
    }

    void Update()
    {
        NetworkManagerMain thePlayer = GameObject.FindObjectOfType<NetworkManagerMain>();
        Debug.Log(thePlayer);
        List<Player> GPlayers = thePlayer.GamePlayers;
        
        foreach (Player player in GPlayers)
        {
            float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
            if (Input.GetKeyDown(KeyCode.E) && dist <= 2.5f)
            {
                Debug.Log("PickUp");
                ItemPickUp script = player.GetComponent<ItemPickUp>();
                script.PickUpItem(EquippedItem.rock);
                Destroy(transform.gameObject);
                Debug.Log(player.GetComponent<NetworkObjectDestroyer>());
                player.GetComponent<NetworkObjectDestroyer>().TellServerToDestroyObject(transform.gameObject);

            }
        }
        Debug.Log(GPlayers.Count);
    }
   
}