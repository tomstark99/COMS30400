using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Testable : MonoBehaviourPun
{
    void Start() {
        // Run the tester
        // AssertTests(Test());
        Test();
    }
    
    public virtual void Test() {
        // Call tests from this override
        Debug.Log("No tests implemented");
    }

    // private void AssertTests(bool test) {
    //     if (test)
    //     {
    //         Debug.Log("All tests passed");
    //     } else 
    //     {
    //         Debug.Log("Testing error");
    //     }
    // }
}
