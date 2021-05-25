using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;

public class TestPlayers : TestClass
{
    GameObject[] players;

    public override IEnumerator InitializeTests()
    {
        ModuleName = "TestPlayers";
        yield return new WaitForSeconds(60f);
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    [Test]
    public IEnumerator TestRightNumberOfPlayers()
    {
        Assert.IsTrue(players.Length > 0 && players.Length <= 2);
        yield return null;
    }


    [Test]
    public IEnumerator TestPlayersHaveRightComponents()
    {
        foreach (var p in players)
        {
            Assert.IsNotNull(p.GetComponent<Animator>());
            Assert.IsNotNull(p.GetComponent<PlayerAnimation>());
            Assert.IsNotNull(p.GetComponent<Objectives>());
            Assert.IsNotNull(p.GetComponent<PlayerMovementPhoton>());
            Assert.IsNotNull(p.GetComponent<PhotonAnimatorView>());
            Assert.IsNotNull(p.GetComponent<PhotonView>());
            Assert.IsNotNull(p.GetComponent<VoiceChatConnector>());
            Assert.IsNotNull(p.GetComponent<SoundRipples>());
            Assert.IsNotNull(p.GetComponent<Achievements>());
            Assert.IsNotNull(p.GetComponent<ArrowGlowing>());
            Assert.IsNotNull(p.GetComponent<MouseLookPhoton>());
            Assert.IsNotNull(p.GetComponent<PlayerOnTrain>());
            Assert.IsNotNull(p.GetComponent<PlayerMovingTrain>());
            Assert.IsNotNull(p.GetComponent<ProximityVoice>());
            Assert.IsNotNull(p.GetComponent<IkBehaviour>());
            Assert.IsNotNull(p.GetComponent<Character>());
            Assert.IsNotNull(p.GetComponent<PhotonPlayer>());
            Assert.IsNotNull(p.GetComponent<ItemInteraction>());
            Assert.IsNotNull(p.GetComponent<FPScounter>());
            Assert.IsNotNull(p.GetComponent<PlayerAudioClips>());
        }
        yield return null;
    }

    [Test]
    public IEnumerator TestPlayersVoiceChatScriptsInitializedCorectly()
    {
        Assert.IsNotNull(GameObject.Find("[PeerJS]VoiceChat"));

        foreach (var p in players)
        {
            Assert.IsNotNull(p.GetComponent<VoiceChatConnector>().audioListener);
            Assert.IsNotNull(p.GetComponent<SoundRipples>().voice);
            Assert.IsNotNull(p.GetComponent<SoundRipples>().particles);
        }
        yield return null;
    }

    [Test]
    public IEnumerator TestPlayersObjectivesSetup()
    {
        foreach(var p in players)
        {
            var script = p.GetComponent<Objectives>();
            Assert.IsNotNull(script.objectivesTitle);
            Assert.IsNotNull(script.objectivesBackground);
            Assert.IsNotNull(script.FindBackpacks);
            Assert.IsNotNull(script.FindTrain);
            Assert.IsNotNull(script.FindBackpacksDesc);
            Assert.IsNotNull(script.FindTrainDesc);
        }
        yield return null;
    }

    [Test]
    public IEnumerator TestPlayersCharacterSetup()
    {
        foreach (var p in players)
        {
            var script = p.GetComponent<Character>();
            Assert.IsNotNull(script.pickUpDestination);
            Assert.IsNotNull(script.pickUpDestinationLocal);
            Assert.IsNotNull(script.dragDestination);
            Assert.IsNotNull(script.grabDestination);
            Assert.IsNotNull(script.bulletPrefab);
            Assert.IsNotNull(script.crosshair);
            Assert.IsNotNull(script.camera);
            Assert.IsNotNull(script.backPackObject);
            Assert.IsNotNull(script.actualCamera);
        }
        yield return null;
    }
}