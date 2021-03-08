using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BreakFencePhoton : MonoBehaviourPun
{
    public GameObject text;
    private GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            if (tempDist <= 2.5f)
            {
                text.SetActive(true);
            }
            else if (tempDist > 2.5f)
            {
                text.SetActive(false);
            }
        }

        foreach (var player in players)
        {
            float tempDist = Vector3.Distance(player.transform.position, transform.position);
            string gesture = player.GetComponent<PhotonPlayer>().gesture;
            bool pPressed = player.GetComponent<PhotonPlayer>().IsPressingP();
            //Debug.Log(gesture);
            if ((gesture.CompareTo("P") == 0 || pPressed) && tempDist <= 2.5f)
            {
                Vector3 spawnPosition = transform.position;
                //gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                PhotonNetwork.Destroy(transform.gameObject);
                PhotonNetwork.Instantiate("PhotonPrefabs/fence_simple_broken_open Variant 1", spawnPosition, Quaternion.Euler(0f, 90f, 0f));
                //NetworkServer.Spawn(brokenFence);
                break;
            }
        }

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    float dist = Vector3.Distance(NetworkClient.connection.identity.transform.position, transform.position);
        //    if (dist <= 2.5f)
        //    {
        //        Vector3 spawnPosition = transform.position;
        //        PhotonNetwork.Destroy(transform.gameObject);
        //        PhotonNetwork.Instantiate("PhotonPrefabs/fence_simple_broken_open Variant 1", spawnPosition, Quaternion.Euler(0f, 90f, 0f));
        //        //NetworkServer.Spawn(brokenFence);
        //    }
        //}
    }
}
