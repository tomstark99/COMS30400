using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHitGroundAlert : MonoBehaviour
{
    public GameObject guards;
    // Start is called before the first frame update
    void Start()
    {
        guards = GameObject.Find("Guards1");
        Debug.Log(guards);
        foreach (Transform child in guards.transform)
        {

            
            float dist = Vector3.Distance(gameObject.transform.position, child.transform.position);
            //Debug.Log(dist);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.position);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "terain")
        {
            Debug.Log("hit terrain");
        }
    }
}
