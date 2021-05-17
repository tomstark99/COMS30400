using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using Photon.Pun;

public class TestGuards
{
    
    public static void Test() {
        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");
        Assert.raiseExceptions = true;
        UnityEngine.Assertions.Assert.raiseExceptions = true;
        testGuardsActive(guards);
        testGuardNavMesh(guards);
        testGuardLobbySettings(guards);
        Debug.Log("Test Guards: 3 tests passed");
    }

    private static void testGuardsActive(GameObject[] guards) {
        Assert.raiseExceptions = true;
        UnityEngine.Assertions.Assert.raiseExceptions = true;
        foreach (var g in guards) {
            // Debug.Assert(!g.activeSelf);
            Assert.IsTrue(!g.activeSelf);
            Assert.IsTrue(g.activeSelf);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().GuardState != GuardAIPhoton.State.Patroling);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().GuardState == GuardAIPhoton.State.Patroling);
        }
    }

    private static void testGuardNavMesh(GameObject[] guards) {
        foreach (var g in guards) {
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().guard == null);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().guard.remainingDistance != null);
        }
    }

    private static void testGuardLobbySettings(GameObject[] guards) {
        foreach (var g in guards) {
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().sightRange == (int)PhotonNetwork.CurrentRoom.CustomProperties["GuardSightRange"]);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().guardAngle == (int)PhotonNetwork.CurrentRoom.CustomProperties["GuardAngle"]);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().speedChasing == (int)PhotonNetwork.CurrentRoom.CustomProperties["SpeedChasing"]);
            Assert.IsTrue(g.GetComponent<GuardAIPhoton>().speedPatrolling == (int)PhotonNetwork.CurrentRoom.CustomProperties["SpeedPatrolling"]);
        }
    }
}
