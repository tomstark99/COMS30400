using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // coroutine when bullet is spawned that destroys the bullet after 3 seconds to not cause spam of bullets in the level
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}
