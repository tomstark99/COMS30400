using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

/*
    This class is used in the LoadingScene and takes care of switching the players over to the new level
*/
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
        // master client sends RPC to all players to load the level
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
        // starts a coroutine that asynchronously loads the level, we get the level to load from the room's custom properties, this level to load is set when we press the StartGame button 
        StartCoroutine(LoadSceneAsync(PhotonNetwork.CurrentRoom.CustomProperties["levelToLoad"].ToString()));
    }

    // asynchronous loading of the level, however due to WebGL being single threaded and not really supporting async loading, this is more of a placeholder loading screen as it doesn't wait for
    // the other player to load the level as we want it to
    IEnumerator LoadSceneAsync(string levelName)
    {
        // start async operation
        AsyncOperation op = SceneManager.LoadSceneAsync(levelName);

        // set scene activation to false and only set it back to true once both players send an RPC claiming they loaded the level
        op.allowSceneActivation = false;

        // while operation is not finished, we keep updaing the loading bar
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);

            // sets the loading bar to the progress (usually just skips to 90 though)
            loadingBar.value = op.progress;
            loadingText.text = 100f * op.progress + "%";

            // once progress is above 90%, we set the map loaded to be true and tell other player we loaded the level
            // Unity recognises 90% to be level loaded, anything above and the game never switches scenes
            if (op.progress >= 0.9f && mapLoaded == false)
            {
                mapLoaded = true;
                photonView.RPC(nameof(AddLoaded), RpcTarget.AllBuffered);
            }
            // WebGL doesnt seem to care about this if statement :(
            // normally on the Unity Editor this makes sure we wait for the other player to load the level before changing scenes 
            // however on WebGl this is completely ignored and the player switched regardless
            if (loadedCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // RPC that increases the loaded count of players
    [PunRPC]
    public void AddLoaded()
    {
       // Debug.Log("loaded player");
        loadedCount += 1;
    }
}
