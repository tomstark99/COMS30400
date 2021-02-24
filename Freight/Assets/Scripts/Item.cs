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

        //get a list of players
        NetworkManagerMain thePlayer = GameObject.FindObjectOfType<NetworkManagerMain>();
        List<Player> GPlayers = thePlayer.GamePlayers;
        
        //check the distance from the item to each player
        foreach (Player player in GPlayers)
        {
            float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

            //if the key E is pressed and the item is near the player
            if (Input.GetKeyDown(KeyCode.E) && dist <= 2.5f)
            {
                //pick up item
                ItemPickUp script = player.GetComponent<ItemPickUp>();
                script.PickUpItem(EquippedItem.rock);

                //destroy the item
                Destroy(transform.gameObject);
                player.GetComponent<NetworkObjectDestroyer>().TellServerToDestroyObject(transform.gameObject);

            }
        }
    }
   
}