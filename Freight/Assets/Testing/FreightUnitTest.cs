using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Photon.Pun;

public class FreightUnitTest : Testable
{
    // [SerializeField]
    // private GameObject guards;

    public override void Test() {
        TestGuards.Test();
        Debug.Log("All Tests Passed");
    }

}
