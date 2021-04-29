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

    GameObject cube;
    GameObject camera;
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

        if((Input.GetKeyDown(KeyCode.E) || PoseParser.GETGestureAsString().CompareTo("B")==0) && photonView.IsMine)
        {
            photonView.RPC("PickUpRock", RpcTarget.All);
       

        }

        if(Input.GetMouseButtonDown(0) && photonView.IsMine)
        {
           

             cube = transform.GetChild(2).gameObject;

             camera = cube.transform.GetChild(0).gameObject;

            photonView.RPC("ThrowRock", RpcTarget.All);
        }
    }

    [PunRPC]
    void PickUpRock()
    {
        
        rock.transform.parent = transform.GetChild(3);
        rock.GetComponent<Rigidbody>().isKinematic = true;
        rock.transform.position = rock.transform.parent.position;
    }

    [PunRPC]
    void ThrowRock()
    {
        rock.transform.parent = rocks.transform;
        rock.GetComponent<Rigidbody>().isKinematic = false;
        rock.transform.position = transform.GetChild(3).position;

        rock.GetComponent<Rigidbody>().AddForce(camera.transform.forward * 1000);
    }

    
}
