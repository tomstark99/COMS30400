using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TestFirstLevel : TestClass
{
    public override IEnumerator InitializeTests()
    {
        ModuleName = "TestFirstLevel";
        yield return null;
    }

    [Test]
    public IEnumerator TestMapHasRightObjects()
    {
        Assert.IsNotNull(GameObject.Find("Songs"));
        Assert.IsNotNull(GameObject.Find("Directional Light"));
        Assert.IsNotNull(GameObject.Find("GameSetup"));
        Assert.IsNotNull(GameObject.Find("SpawnPoints"));
        Assert.IsNotNull(GameObject.Find("LeavingTrains"));
        Assert.IsNotNull(GameObject.Find("Endgame"));
        Assert.IsNotNull(GameObject.Find("Level"));
        Assert.IsNotNull(GameObject.Find("Terrain"));
        Assert.IsNotNull(GameObject.Find("Borders"));
        Assert.IsNotNull(GameObject.Find("NavMesh Surface"));
        Assert.IsNotNull(GameObject.Find("SyncedTime"));
        Assert.IsNotNull(GameObject.Find("EventSystem"));
        Assert.IsNotNull(GameObject.Find("PostProcessing"));
        Assert.IsNotNull(GameObject.Find("PostProcessingWater"));
        Assert.IsNotNull(GameObject.Find("CM vcam1"));
        Assert.IsNotNull(GameObject.Find("CameraToSeeTheLights"));
        Assert.IsNotNull(GameObject.Find("Camera"));
        Assert.IsNotNull(GameObject.Find("DestroyArrows"));
        yield return null;
    }
}