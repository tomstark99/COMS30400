using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMovement : MonoBehaviour
{
    public Transform[] target;
    public float speed;
    public float rotationSpeed;
    private int current;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position != target[current].position) {
            Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, speed * Time.deltaTime);
            Vector3 rot = Vector3.RotateTowards(transform.forward, (transform.position - target[current].position), rotationSpeed * Time.deltaTime, 0.0f);
            // transform.position = pos;
            // transform.rotation = Quaternion.LookRotation(rot);
            GetComponent<Rigidbody>().MovePosition(pos);
            // GetComponent<Rigidbody>().RotatePosition(rot);
        } else {
            current = (current + 1) % target.Length;
        }
    }
}
