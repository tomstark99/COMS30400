using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles);
        }
    }
}
