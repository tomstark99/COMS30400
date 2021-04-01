using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateLight : MonoBehaviour
{
    [SerializeField]
    private GameObject spotlight;
    private bool positiveRotation;
    private float rotationMultiplier = 10f;
    public float rotationUpperLimit;
    public float rotationLowerLimit;

    // Start is called before the first frame update
    void Start()
    {
        positiveRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(spotlight.transform.eulerAngles.y);
        if (positiveRotation == true)
        {
            spotlight.transform.Rotate(Vector3.up * rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y < rotationUpperLimit && spotlight.transform.eulerAngles.y > rotationLowerLimit)
            {
                positiveRotation = false;
            }
        }
        else
        {
            spotlight.transform.Rotate(Vector3.up * -rotationMultiplier * Time.deltaTime);
            if (spotlight.transform.eulerAngles.y < rotationUpperLimit && spotlight.transform.eulerAngles.y > rotationLowerLimit)
            {
                positiveRotation = true;
            }
        }
    }
}
