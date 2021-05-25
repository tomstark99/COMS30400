using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using Photon.Pun;
using Smooth;

public class TestGuards : TestClass
{

    GameObject[] guards;

    public override IEnumerator InitializeTests() {
        ModuleName = "TestGuards";
        yield return new WaitForSeconds(10f); 
        guards = GameObject.FindGameObjectsWithTag("Guard");
    }
    
    [Test]
    public IEnumerator TestGuardsHaveRightComponents()
    {
        foreach (var g in guards)
        {
            Assert.IsNotNull(g.GetComponent<GuardAIPhoton>());
            Assert.IsNotNull(g.GetComponent<NavMeshAgent>());
            Assert.IsNotNull(g.GetComponent<GuardAnimation>());
            Assert.IsNotNull(g.GetComponent<PhotonView>());
            Assert.IsNotNull(g.GetComponent<SmoothSyncPUN2>());
            Assert.IsNotNull(g.GetComponent<PhotonAnimatorView>());
            Assert.IsNotNull(g.GetComponent<Animator>());
            Assert.IsNotNull(g.GetComponent<CapsuleCollider>());
        }
        yield return null;
    }

    [Test]
    public IEnumerator TestGuardAIPhotonScriptHasRightFields()
    {
        foreach (var g in guards)
        {
            GuardAIPhoton script = g.GetComponent<GuardAIPhoton>();
            Assert.IsNotNull(script.guard);
            Assert.IsNotNull(script.spotlight);
            Assert.IsNotNull(script.sounds);
        }
        yield return null;
    }

    private static void testGuardsActive(GameObject[] guards)
    {
        Assert.raiseExceptions = true;
        UnityEngine.Assertions.Assert.raiseExceptions = true;
        foreach (var g in guards)
        {
            // Debug.Assert(!g.activeSelf);
            Debug.Log(g.activeSelf);
            Assert.IsTrue(!g.activeSelf);
            Assert.IsTrue(g.activeSelf);
            // Assert.IsTrue(g.GetComponent<GuardAIPhoton>().GuardState != GuardAIPhoton.State.Patroling);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().GuardState == GuardAIPhoton.State.Patroling);
        }
    }

        [Test]
    public IEnumerator TestGuardsActive() {
        foreach (var g in guards) {
            Assert.IsTrue(g.activeSelf);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().GuardState == GuardAIPhoton.State.Patroling);
        }
        yield return null;
    }
   
    [Test]
    public IEnumerator TestGuardLobbySettings() {
        foreach (var g in guards) {
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().sightRange == (int)PhotonNetwork.CurrentRoom.CustomProperties["GuardSightRange"]);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().guardAngle == (int)PhotonNetwork.CurrentRoom.CustomProperties["GuardAngle"]);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().speedChasing == (int)PhotonNetwork.CurrentRoom.CustomProperties["SpeedChasing"]);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().speedPatrolling == (int)PhotonNetwork.CurrentRoom.CustomProperties["SpeedPatrolling"]);
        }
        yield return null;
    }
}
