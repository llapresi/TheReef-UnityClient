using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadIntro : MonoBehaviour {

    public TimeManager timer;
    public ItemSpawning items;
    public CoralManaging coral;

    // Use this for initialization
    void Start () {
        BeginIntro();
    }
	
	public void BeginGame()
    {
        // Called by StartingScene when it finished to actually start the game
        timer.StartTimer();
        items.ResetTrash();
    }

    public void EndGame()
    {
        items.StopSpawningTrash();
        StartCoroutine(LoadScene("EndingScene"));
    }

    public void BeginIntro()
    {
        StartCoroutine(LoadScene("StartingSceneAnimationOnly"));
    }


    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Do stuff after scene loads here
            yield return null;
        }
    }
}
