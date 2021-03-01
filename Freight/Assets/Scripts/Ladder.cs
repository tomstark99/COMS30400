using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            GameObject player = other.gameObject;
            Vector3 move = player.transform.up * 0.1f;
            player.GetComponent<CharacterController>().Move(move * 10f * Time.deltaTime);
            // other.gameObject.GetComponent<CharacterController>().Move()
        }
    }
}
