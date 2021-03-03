using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class ItemPickUpPhoton : MonoBehaviourPun
{
    public GameObject rocks;
    public GameObject rockPrefab;
    private GameObject rock;
    // Start is called before the first frame update
    void Start()
    {

        rocks = GameObject.Find("Rocks");
        rock = rocks.transform.GetChild(0).gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        rocks = GameObject.Find("Rocks");

       

        if(Input.GetKeyDown(KeyCode.E) && photonView.IsMine)
        {
            photonView.RPC("PickUpRock", RpcTarget.All);
       

        }
    }

    [PunRPC]
    void PickUpRock()
    {
        
        rock.transform.parent = transform.GetChild(3);
        rock.GetComponent<Rigidbody>().isKinematic = true;
        rock.transform.position = rock.transform.parent.position;
    }

    
}
