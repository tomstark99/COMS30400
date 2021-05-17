using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Achievements : MonoBehaviourPun
{
    [SerializeField]
    private GameObject achievements;
    [SerializeField]
    private GameObject achievementsTab;

    // first row 
    private GameObject babySteps;
    private GameObject letTheHuntBegin;
    private GameObject peaceTreaty;
    private GameObject useNature;
    private GameObject roadman;
    private GameObject theCompletePicture;

    // second row
    private GameObject likeANinja;
    private GameObject hackerman;
    private GameObject learnTheHardWay;
    private GameObject chooChoo;
    private GameObject onTheRun;
    private GameObject freight;

    [SerializeField]
    private GameObject mapMask;
    [SerializeField]
    private GameObject border;

    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

    // this is a checker 
    public bool wasDetectedOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            var tempColor = achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            tempColor.a = 1f;

            // first row 
            if (PlayerPrefs.HasKey("BabySteps1"))
            {
                achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("LetTheHuntBegin1"))
            {
                achievementsTab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("PeaceTreaty1"))
            {
                achievementsTab.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("UseNature1"))
            {
                achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("Roadman1"))
            {
                achievementsTab.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("TheCompletePicture1"))
            {
                achievementsTab.transform.GetChild(0).GetChild(5).GetComponent<Image>().color = tempColor;
            }

            // second row
            if (PlayerPrefs.HasKey("LikeANinja1"))
            {
                achievementsTab.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("Hackerman1"))
            {
                achievementsTab.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("LearnTheHardWay1"))
            {
                achievementsTab.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("ChooChoo1"))
            {
                achievementsTab.transform.GetChild(1).GetChild(3).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("OnTheRun1"))
            {
                achievementsTab.transform.GetChild(1).GetChild(4).GetComponent<Image>().color = tempColor;
            }
            if (PlayerPrefs.HasKey("Freight1"))
            {
                achievementsTab.transform.GetChild(1).GetChild(5).GetComponent<Image>().color = tempColor;
            }
            babySteps = achievements.transform.GetChild(0).gameObject;
            letTheHuntBegin = achievements.transform.GetChild(1).gameObject;
            peaceTreaty = achievements.transform.GetChild(2).gameObject;
            useNature = achievements.transform.GetChild(3).gameObject;
            roadman = achievements.transform.GetChild(4).gameObject;
            theCompletePicture = achievements.transform.GetChild(5).gameObject;

            likeANinja = achievements.transform.GetChild(6).gameObject;
            hackerman = achievements.transform.GetChild(7).gameObject;
            learnTheHardWay = achievements.transform.GetChild(8).gameObject;
            chooChoo = achievements.transform.GetChild(9).gameObject;
            onTheRun = achievements.transform.GetChild(10).gameObject;
            freight = achievements.transform.GetChild(11).gameObject;

            StartCoroutine(CoroutineCoordinator());
        }
    }

    // this function makes sure that achievments appear one after another instead of at the same time
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }

    public void WasDetected()
    {
        if (!wasDetectedOnce)
            wasDetectedOnce = true;
    }

    public void BabyStepsCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("BabySteps1"))
            {
                PlayerPrefs.SetInt("BabySteps1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = tempColor;
                //StartCoroutine(BabyStepsSequence());
                coroutineQueue.Enqueue(BabyStepsSequence());
            }

        }
    }

    IEnumerator BabyStepsSequence()
    {
        babySteps.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        babySteps.SetActive(false);
    }

    public void LetTheHuntBeginCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("LetTheHuntBegin1"))
            {
                PlayerPrefs.SetInt("LetTheHuntBegin1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = tempColor;
                //StartCoroutine(LetTheHuntBeginSequence());
                coroutineQueue.Enqueue(LetTheHuntBeginSequence());
            }

        }
    }

    IEnumerator LetTheHuntBeginSequence()
    {
        letTheHuntBegin.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        letTheHuntBegin.SetActive(false);
    }

    public void PeaceTreatyCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("PeaceTreaty1"))
            {
                PlayerPrefs.SetInt("PeaceTreaty1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(2).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = tempColor;
                //StartCoroutine(LetTheHuntBeginSequence());
                coroutineQueue.Enqueue(PeaceTreatySequence());
            }

        }
    }

    IEnumerator PeaceTreatySequence()
    {
        peaceTreaty.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        peaceTreaty.SetActive(false);
    }

    public void UseNatureCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("UseNature1"))
            {
                PlayerPrefs.SetInt("UseNature1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = tempColor;
                //StartCoroutine(UseNatureSequence());
                coroutineQueue.Enqueue(UseNatureSequence());
            }
        }
    }

    IEnumerator UseNatureSequence()
    {
        useNature.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        useNature.SetActive(false);
    }

    public void Roadman()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("Roadman1"))
            {
                PlayerPrefs.SetInt("Roadman1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(4).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(RoadmanSequence());
            }
        }
    }

    IEnumerator RoadmanSequence()
    {
        roadman.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        roadman.SetActive(false);
    }

    public void TheCompletePictureCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("TheCompletePicture1"))
            {
                PlayerPrefs.SetInt("TheCompletePicture1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(0).GetChild(5).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(0).GetChild(5).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(TheCompletePictureSequence());
            }
        }
    }

    IEnumerator TheCompletePictureSequence()
    {
        theCompletePicture.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        theCompletePicture.SetActive(false);
    }

    public void HackermanCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("Hackerman1"))
            {
                PlayerPrefs.SetInt("Hackerman1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(1).GetChild(1).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(HackermanSequence());
            }
        }
    }

    IEnumerator HackermanSequence()
    {
        hackerman.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        hackerman.SetActive(false);
    }

    public void LearnTheHardWayCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("LearnTheHardWay1"))
            {
                PlayerPrefs.SetInt("LearnTheHardWay1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(1).GetChild(2).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(LearnTheHardWaySequence());
            }
        }
    }

    IEnumerator LearnTheHardWaySequence()
    {
        learnTheHardWay.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        learnTheHardWay.SetActive(false);
    }

    public void ChooChooCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("ChooChoo1"))
            {
                PlayerPrefs.SetInt("ChooChoo1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(1).GetChild(3).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(1).GetChild(3).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(ChooChooSequence());
            }
        }
    }

    IEnumerator ChooChooSequence()
    {
        chooChoo.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        chooChoo.SetActive(false);
    }

    public void OnTheRunCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("OnTheRun1"))
            {
                PlayerPrefs.SetInt("OnTheRun1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(1).GetChild(4).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(1).GetChild(4).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(OnTheRunSequence());
            }
        }
    }

    IEnumerator OnTheRunSequence()
    {
        onTheRun.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        onTheRun.SetActive(false);
    }

    public void LikeANinjaCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("LikeANinja1") && !wasDetectedOnce)
            {
                PlayerPrefs.SetInt("LikeANinja1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(LikeANinjaSequence());
            }
        }
    }

    IEnumerator LikeANinjaSequence()
    {
        likeANinja.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        likeANinja.SetActive(false);
    }

    public void FreightCompleted()
    {
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("Freight1"))
            {
                PlayerPrefs.SetInt("Freight1", 1);
                PlayerPrefs.Save();
                var tempColor = achievementsTab.transform.GetChild(1).GetChild(5).GetComponent<Image>().color;
                tempColor.a = 1f;
                achievementsTab.transform.GetChild(1).GetChild(5).GetComponent<Image>().color = tempColor;
                //StartCoroutine(RoadmanSequence());
                coroutineQueue.Enqueue(FreightSequence());
            }
        }
    }

    IEnumerator FreightSequence()
    {
        freight.SetActive(true);
        mapMask.SetActive(false);
        border.SetActive(false);
        yield return new WaitForSeconds(3f);
        border.SetActive(true);
        mapMask.SetActive(true);
        freight.SetActive(false);
    }
}
