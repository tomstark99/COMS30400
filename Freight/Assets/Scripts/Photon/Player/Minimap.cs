using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Photon.Pun;

public class Minimap : MonoBehaviour
{
    private Transform player;
    // private GameObject[] bags;
    // private GameObject[] players;

    void Start() {
        player = transform.parent;
    }
    // Start is called before the first frame update
    void Update() {
        Vector3 newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0.0f);
    }

   
}
