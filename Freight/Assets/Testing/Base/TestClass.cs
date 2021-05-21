using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Reflection;

public abstract class TestClass : MonoBehaviourPun
{
    //Tests that are run before after the first frame and after InitializeTests
    public class Test : Attribute 
    {
    }

    public string ModuleName;

    private int testMethods;

    IEnumerator Start()
    {
        yield return StartCoroutine(InitializeTests());

        yield return null;

        testMethods = 0;
        foreach (MethodInfo method in this.GetType().GetMethods())
        {
            if (method.GetCustomAttribute<Test>() != null)
            {
                StartCoroutine(method.Name);
                testMethods++;
            }

        }

        Debug.Log("All " + testMethods + " tests passed for: " + this.ModuleName);
    }


    public virtual IEnumerator InitializeTests() {
        yield return null;
    }

}
