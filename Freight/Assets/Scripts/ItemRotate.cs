using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemRotate : MonoBehaviour
{
    [SerializeField]
    private GameObject item;
    private float rotationMultiplier;
    private float bounceMultiplier;  
    private float originalYPos;

    // Start is called before the first frame update
    void Start()
    {
        rotationMultiplier = 15.0f;
        bounceMultiplier = 0.15f;
        originalYPos = item.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        item.transform.Rotate(Vector3.up * rotationMultiplier * Time.deltaTime);
        Vector3 pos = new Vector3(item.transform.position.x, originalYPos + (Mathf.Sin(Time.time) * bounceMultiplier), item.transform.position.z);
        item.transform.position = pos;
    }
}
