using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovesScript : MonoBehaviour
{
    Vector3 aPos;
    float movementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);

    }
}
