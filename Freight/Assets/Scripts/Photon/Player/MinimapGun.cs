using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Photon.Pun;

public class MinimapGun : MonoBehaviour
{
    private Transform player;
    // private GameObject[] bags;
    // private GameObject[] players;

    void Start() {
        player = transform.parent;
        // players = GameObject.FindGameObjectsWithTag("Player");
        // bags = GameObject.FindGameObjectsWithTag("Bag");
        // foreach (var bag in bags)
        // {
            // bag.GetComponent<Grabbable>().BagPickedUp += RemoveBagFromMinimap;
        // }
    }
    // Start is called before the first frame update
    void Update() {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 180.0f);
    }

    // private void RemoveBagFromMinimap() {
    //     foreach (var player in players)
    //     {
    //         // if(player.)
    //     }
    // }
}
