using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InitialText : MonoBehaviour
{
    public GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestruct());
        // StartCoroutine(CameraMove());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        camera.SetActive(true);
    }

    // IEnumerator CameraMove()
    // {
    // }
}
