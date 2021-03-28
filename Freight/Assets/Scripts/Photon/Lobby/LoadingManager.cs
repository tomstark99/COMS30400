using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviourPun
{
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    private bool mapLoaded;

    private int loadedCount;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(LoadLevel), RpcTarget.All);
        }

        mapLoaded = false;
        loadedCount = 0;
    }

    [PunRPC]
    public void LoadLevel()
    {
        StartCoroutine(LoadSceneAsync("Assets/Scenes/TrainStationPun.unity"));
    }

    IEnumerator LoadSceneAsync(string levelName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(levelName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            Debug.Log(op.progress);

            loadingBar.value = op.progress;
            loadingText.text = 100f * op.progress + "%";

            if (op.progress >= 0.9f && mapLoaded == false)
            {
                mapLoaded = true;
                photonView.RPC(nameof(AddLoaded), RpcTarget.AllBufferedViaServer);
            }
            if (loadedCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    [PunRPC]
    public void AddLoaded()
    {
        Debug.Log("loaded player");
        loadedCount += 1;
    }
}
