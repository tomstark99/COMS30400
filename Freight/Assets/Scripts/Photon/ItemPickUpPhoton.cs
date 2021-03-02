using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemPickUpPhoton : MonoBehaviourPun
{
    public GameObject rocks;

    // Start is called before the first frame update
    void Start()
    {
        rocks = GameObject.Find("Rocks");
    }

    // Update is called once per frame
    void Update()
    {
        rocks = GameObject.Find("Rocks");

        foreach (Transform rock in rocks.transform)
        {
            float tempDist = Vector3.Distance(transform.position, rock.position);
            if (tempDist <= 5f && Input.GetKeyDown(KeyCode.F))
            {
                rock.parent = transform.GetChild(3);
                rock.GetComponent<Rigidbody>().isKinematic = true;
                rock.GetComponent<Rigidbody>().useGravity = false;
                rock.transform.position = rock.parent.position;
            }
        }
    }
}
