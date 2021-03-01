using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstantiatePoles : MonoBehaviour
{
    public GameObject poles;
    private Vector3 positionStart = new Vector3(370.0f, 8.5f, 210.0f);
    private Vector3 position = new Vector3(370.0f, 8.5f, 210.0f);

    // Start is called before the first frame update
    void Start()
    {
        position = positionStart;
        for (int i = 0; i < 4; i++)
        {
            if(i==0){
                position.z += 20.0f;
            } else{
                position.z -= (float)Math.Cos(Math.PI/4)*20.0f;
                position.x += (float)Math.Cos(Math.PI/4)*20.0f;
            }
            Instantiate(poles, position, Quaternion.Euler(0f, 135f, 0f));
        }
        position = positionStart;
        for (int i = 0; i < 9; i++)
        {
            position.z += 20.0f;
            Instantiate(poles, position, Quaternion.Euler(0f, 0f, 0f));
        }
        for (int i = 0; i < 3; i++)
        {
            if(i==0){
                position.z += 20.0f;
            } else{
                position.z += (float)Math.Cos(Math.PI/4)*20.0f;
                position.x -= (float)Math.Cos(Math.PI/4)*20.0f;
            }
            Instantiate(poles, position, Quaternion.Euler(0f, -45f, 0f));
        }
        Debug.Log("Poles");
    }
}
