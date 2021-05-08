using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BagSpawner : MonoBehaviourPun
{
    [SerializeField]
    private Transform[] spawnPoints;

    public Transform backpacks;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                SpawnOneBag();
            } 
            else
            {
                StartCoroutine(SpawnTwoBags());
            }
        }

    }

    void SpawnOneBag()
    {
        int index = Random.Range(0, spawnPoints.Length-1);
        GameObject bag = PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Backpack-20L_i", spawnPoints[index].position, Quaternion.identity);
        bag.transform.parent = backpacks;
    }

    [PunRPC]
    void BagParentRPC(int bagID, int bag2ID)
    {
        GameObject bag = PhotonView.Find(bagID).gameObject;
        GameObject bag2 = PhotonView.Find(bag2ID).gameObject;

        bag.transform.parent = backpacks;
        bag2.transform.parent = backpacks;
    }

    IEnumerator SpawnTwoBags()
    {
        yield return new WaitForSeconds(6f);
        bool spawned = false;
        while (!spawned)
        {
            int index1 = Random.Range(0, spawnPoints.Length);
            int index2 = Random.Range(0, spawnPoints.Length);
            // this is to make sure the spawn points are different
            if (index1 != index2)
            {
                GameObject bag = PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Backpack-20L_i", spawnPoints[index1].position, Quaternion.identity);
                
                GameObject bag2 = PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/Backpack-20L_i", spawnPoints[index2].position, Quaternion.identity);

                photonView.RPC(nameof(BagParentRPC), RpcTarget.AllBufferedViaServer, bag.GetComponent<PhotonView>().ViewID, bag2.GetComponent<PhotonView>().ViewID);

                spawned = true;
                
            }
            yield return null;
        }
         
    }
}
