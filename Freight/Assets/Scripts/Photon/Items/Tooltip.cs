using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
    Class used to create a temporary tooltip above an object that destroys itself after 5 seconds 
*/
public class Tooltip : MonoBehaviour
{
    private GameObject player;
    public GameObject Player
    {
        set { player = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // self destruct after 5 seconds
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        // rotates the tooltip to face the player 
        if (player)
        {
            transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles);
        }
    }
}
